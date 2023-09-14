using Domain;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Repository.Interfaces;
using TaskManager.Application.Service.Interfaces;
using TaskManager.Domain.Dtos;

namespace TaskManager.Infrastructure.Services
{
    public class UserService: IUserService
    {
        private readonly IRepositoryManager _repository;

        public UserService(IRepositoryManager repository)
        {
            this._repository = repository;
        }

        public async Task<GenericResponse<Response>> CreateUser(RegisterDto registerUser)
        {
            try
            {
                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if (string.IsNullOrEmpty(registerUser.Name) || string.IsNullOrEmpty(registerUser.Password) || string.IsNullOrEmpty(registerUser.Email))
                    return new GenericResponse<Response>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Please, enter all fields",
                    };

                //  CHECK IF EMAIL FORMAT IS CORRECT
                if(!Util.EmailIsValid(registerUser.Email))
                    return new GenericResponse<Response>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "email is in bad format",
                    };

                //  CHECK IF PASSWORD LENGTH IS ABOVE FOUR CHARS
                if(registerUser.Password.Length < 5)
                    return new GenericResponse<Response>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Password must be at least 5 letters",
                        Data = null
                    };

                //  CHECK IF USER ALREADY EXIST
                var isEmailExist = await _repository.UserRepository.GetByEmail(registerUser.Email, false);
                if (isEmailExist != null)
                    return new GenericResponse<Response>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Sorry, email already exist",
                        Data = null
                    };

                var hashedPassword = Util.StringHasher(registerUser.Password);
                User userToSave = new User
                {
                    UserId = Guid.NewGuid(),
                    Name = registerUser.Name,
                    Email = registerUser.Email,
                    Password = hashedPassword,
                    CreatedAt = DateTime.UtcNow.ToString()
                };
                _repository.UserRepository.CreateUser(userToSave);
                await _repository.SaveAsync();

                return new GenericResponse<Response>
                {
                    IsSuccessful = true,
                    ResponseCode = "201",
                    ResponseMessage = "You just successfully created a new user",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Response>
                {
                    IsSuccessful = false,
                    ResponseCode = "400",
                    ResponseMessage = "Error occured while creating your new user",
                    Data = null
                };
            }
        }

        public async Task<GenericResponse<Response>> DeleteUser(string userId)
        {
            try
            {
                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if (string.IsNullOrEmpty(userId))
                    return new GenericResponse<Response>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Please, enter the user Id",
                    };

                Guid userGuid = new Guid(userId);
                var checkIfUserExist = await _repository.UserRepository.GetByUserId(userGuid, true);

                //  CHECK IF THE USER EXIST
                if (checkIfUserExist == null)
                    return new GenericResponse<Response>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "User not found",
                    };

                _repository.UserRepository.DeleteUser(checkIfUserExist);
                await _repository.SaveAsync();

                return new GenericResponse<Response>
                {
                    IsSuccessful = true,
                    ResponseCode = "200",
                    ResponseMessage = "Successfully deleted your user from the database",
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Response>
                {
                    IsSuccessful = false,
                    ResponseCode = "400",
                    ResponseMessage = "Error occured while deleting your user",
                };
            }
        }

        public async Task<GenericResponse<IEnumerable<UserWithIdDto>>> GetAllUsers()
        {
            try
            {
                // THIS WILL GET ALL TASKS FROM THE REPOSITORY
                var allUsers = await _repository.UserRepository.GetUsers();

                //  CHECK IF THE LIST IS EMPTY
                if (allUsers.Count() == 0)
                    return new GenericResponse<IEnumerable<UserWithIdDto>>
                    {
                        IsSuccessful = true,
                        ResponseCode = "200",
                        ResponseMessage = "No user found",
                    };
                List<UserWithIdDto> response = new List<UserWithIdDto>();
                //  THIS IS THE RESPONSE DATA TO SEND BACK TO OUR CONSUMER
                foreach (var user in allUsers)
                {
                    response.Add(new UserWithIdDto()
                    {
                        UserId = user.UserId.ToString(),
                        Name = user.Name,
                        Email = user.Email,
                        RegisteredDate = user.CreatedAt
                    });
                }

                return new GenericResponse<IEnumerable<UserWithIdDto>>
                {
                    IsSuccessful = true,
                    ResponseCode = "200",
                    ResponseMessage = "Successfully fetched all users. Total number: " + allUsers.Count(),
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<IEnumerable<UserWithIdDto>>
                {
                    IsSuccessful = false,
                    ResponseCode = "400",
                    ResponseMessage = "Error occured while getting users",
                    Data = null
                };
            }
        }

        public async Task<GenericResponse<UserDto>> GetByUserId(string userIdString)
        {
            try
            {
                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if (string.IsNullOrEmpty(userIdString))
                    return new GenericResponse<UserDto>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Kindly enter your user Id in the query string",
                    };

                // THIS WILL GET ALL TASKS FROM THE REPOSITORY
                Guid userIdGuid = new Guid(userIdString);
                var getUserFromDb = await _repository.UserRepository.GetByUserId(userIdGuid, false);

                //  CHECK IF USER EXIST
                if (getUserFromDb == null)
                    return new GenericResponse<UserDto>
                    {
                        IsSuccessful = true,
                        ResponseCode = "200",
                        ResponseMessage = "User not found",
                    };

                //  FETCH ALL ASSIGNED TASKED BASED OF THE PRODUCT ID WHICH IS OUR FOREIGN KEY
                var getAssignedTasks = await _repository.TaskRepository.GetTasksByUserId(userIdGuid, false);

                //  MAP THE REQUIRED DATA WE NEED THE USERS TO SEE INTO A NEW DTO WHICH IS THE TASKDTO FOR THE RESPONSE
                List<TaskDto> assignedTasksDto = new List<TaskDto>();
                foreach (var task in getAssignedTasks)
                {
                    assignedTasksDto.Add(new TaskDto
                    {
                        Title = task.Title,
                        Description = task.Description,
                        DueDate = task.DueDate,
                        Priority = task.Priority,
                        Status = task.Status
                    });
                }

                //  THIS IS THE RESPONSE DATA TO SEND BACK TO OUR CONSUMER
                var response = new UserDto()
                {
                    Name = getUserFromDb.Name,
                    Email = getUserFromDb.Email,
                    AssociatedTasks = assignedTasksDto.ToArray()
                };

                return new GenericResponse<UserDto>
                {
                    IsSuccessful = true,
                    ResponseCode = "200",
                    ResponseMessage = "Successfully fetched user",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<UserDto>
                {
                    IsSuccessful = false,
                    ResponseCode = "400",
                    ResponseMessage = "Error occured while getting your user",
                };
            }
        }

        public async Task<GenericResponse<Response>> UpdateUser(string userIdString, UpdateUserRequest request)
        {
            try
            {
                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if (string.IsNullOrEmpty(userIdString))
                    return new GenericResponse<Response>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Kindly enter your user Id in the query string",
                    };

                Guid userIdGuid = new Guid(userIdString);
                var checkIfUserExist = await _repository.UserRepository.GetByUserId(userIdGuid, true);

                //  CHECK IF THE USER EXIST
                if (checkIfUserExist == null)
                    return new GenericResponse<Response>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "User not found",
                    };

                checkIfUserExist.Name = request.Name;
                _repository.UserRepository.UpdateUser(checkIfUserExist);
                await _repository.SaveAsync();


                return new GenericResponse<Response>
                {
                    IsSuccessful = true,
                    ResponseCode = "200",
                    ResponseMessage = "Successfully updated your information",
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Response>
                {
                    IsSuccessful = false,
                    ResponseCode = "400",
                    ResponseMessage = "Error occured while updating your project",
                };
            }
        }

        public async Task<GenericResponse<Response>> AssignTask(string userId, AddOrDelete operation, string taskId)
        {
            try
            {
                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(taskId))
                    return new GenericResponse<Response>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Kindly enter all field",
                        Data = null
                    };

                //  CHECK IF USER EXIST IN DATABASE
                Guid userIdGuid = new Guid(userId);
                var getUserFromDb = await _repository.UserRepository.GetByUserId(userIdGuid, true);
                if (getUserFromDb == null)
                    return new GenericResponse<Response>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "User not exist",
                        Data = null
                    };

                //  CHECK IF TASK EXIST IN DATABASE
                Guid taskIdGuid = new Guid(taskId);
                var checkIfTaskExistInUserTaskDb = await _repository.TaskRepository.GetTaskByTaskId(taskIdGuid, false);

                if (checkIfTaskExistInUserTaskDb == null)
                {
                    return new GenericResponse<Response>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "The task you just assigned does not exist",
                        Data = null
                    };
                }
                else
                {
                    //  TRANSFER ALL CURRENT TASKS IN THE USER INTO A NEW VARIABLE SO IT WILL BE MANIPULATED EASILY
                    List<UserTask> getUserTasks = (List<UserTask>)await _repository.TaskRepository.GetTasksByUserId(userIdGuid, true);

                    //  THIS CONDITION WILL CHECK IF WE NEED TO ADD OR DELETE THE TASK
                    if (operation == AddOrDelete.Add)
                    {
                        //  CHECK IF THE TASK TO ADD ALREADY EXIST
                        if (Util.IsListContainTask(getUserTasks, checkIfTaskExistInUserTaskDb))
                        {
                            return new GenericResponse<Response>
                            {
                                IsSuccessful = false,
                                ResponseCode = "400",
                                ResponseMessage = "This task exist in your profile already",
                            };
                        }
                        else
                        {
                            //  ADD THE NEW TASK TO THE ARRAY AND SAVE TO THE DB
                            getUserTasks.Add(checkIfTaskExistInUserTaskDb);
                        }
                    }
                    else if (operation == AddOrDelete.Delete)
                    {
                        //  CHECK IF THE TASK TO ADD ALREADY EXIST
                        if (Util.IsListContainTask(getUserTasks, checkIfTaskExistInUserTaskDb))
                        {
                            //  ADD THE NEW TASK TO THE ARRAY AND SAVE TO THE DB
                            getUserTasks.RemoveAll(x => x.TaskId == checkIfTaskExistInUserTaskDb.TaskId);
                        }
                        else
                        {
                            return new GenericResponse<Response>
                            {
                                IsSuccessful = false,
                                ResponseCode = "400",
                                ResponseMessage = "This task does not exist in your profile",
                            };
                        }
                    }

                    getUserFromDb.UserTasks = getUserTasks;
                    _repository.UserRepository.UpdateUser(getUserFromDb);
                    await _repository.SaveAsync();

                }
                return new GenericResponse<Response>
                {
                    IsSuccessful = true,
                    ResponseCode = "200",
                    ResponseMessage = "User task have been successfully updated",
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Response>
                {
                    IsSuccessful = false,
                    ResponseCode = "400",
                    ResponseMessage = "Error occured while adding new task to this user",
                };
            }
        }
    }
}
