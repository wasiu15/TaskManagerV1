using Domain;
using Domain.Models;
using TaskManager.Application.Repository.Interfaces;
using TaskManager.Application.Service.Interfaces;
using TaskManager.Domain.Dtos;

namespace TaskManager.Infrastructure.Services
{
    public class UserService: IUserService
    {
        private readonly IRepositoryManager _repository;
        private readonly ITokenManager _tokenManager;

        public UserService(IRepositoryManager repository, ITokenManager tokenManager)
        {
            _repository = repository;
            _tokenManager = tokenManager;
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
                        ResponseMessage = "Please, enter all required fields",
                    };

                //  CHECK IF EMAIL FORMAT IS CORRECT
                if(!Util.EmailIsValid(registerUser.Email))
                    return new GenericResponse<Response>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Email is in bad format",
                    };

                //  CHECK IF PASSWORD LENGTH IS ABOVE FOUR CHARS
                if(registerUser.Password.Length < 5)
                    return new GenericResponse<Response>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Password must be at least 5 letters",
                    };

                //  CHECK IF USER ALREADY EXIST
                var isEmailExist = await _repository.UserRepository.GetByEmail(registerUser.Email, false);
                if (isEmailExist != null)
                    return new GenericResponse<Response>
                    {
                        IsSuccessful = false,
                        ResponseCode = "409",
                        ResponseMessage = "Sorry, email already exist",
                    };

                var hashedPassword = Util.StringHasher(registerUser.Password);
                User userToSave = new User
                {
                    UserId = Guid.NewGuid().ToString(),
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
                    ResponseMessage = "User created successfully",
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Response>
                {
                    IsSuccessful = false,
                    ResponseCode = "500",
                    ResponseMessage = "Error occured while creating new user",
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
                        ResponseMessage = "Please, enter the User ID",
                    };

                var checkIfUserExist = await _repository.UserRepository.GetByUserId(userId, true);

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
                    ResponseMessage = "User deleted successfully",
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Response>
                {
                    IsSuccessful = false,
                    ResponseCode = "500",
                    ResponseMessage = "Error occured while deleting user",
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
                    ResponseMessage = "Users fetched successfully. Total number: " + allUsers.Count(),
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<IEnumerable<UserWithIdDto>>
                {
                    IsSuccessful = false,
                    ResponseCode = "500",
                    ResponseMessage = "Error occured while fetching users",
                };
            }
        }

        public async Task<GenericResponse<UserDto>> GetByUserId(string userId)
        {
            try
            {
                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if (string.IsNullOrEmpty((string)userId))
                    return new GenericResponse<UserDto>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Please, enter your User ID",
                    };

                var getUserFromDb = await _repository.UserRepository.GetByUserId(userId, false);

                //  CHECK IF USER EXIST
                if (getUserFromDb == null)
                    return new GenericResponse<UserDto>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "User not found",
                    };

                //  FETCH ALL ASSIGNED TASKED BASED OF THE PRODUCT ID WHICH IS OUR FOREIGN KEY
                var getAssignedTasks = await _repository.TaskRepository.GetTasksByUserId(userId, false);

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
                    ResponseMessage = "User fetched successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<UserDto>
                {
                    IsSuccessful = true,
                    ResponseCode = "500",
                    ResponseMessage = "Error occured while fetching your user",
                };
            }
        }

        public async Task<GenericResponse<LoginResponse>> GetByEmailAndPassword(LoginDto loginRequest, bool trackChanges)
        {
            try
            {
                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if (string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
                    return new GenericResponse<LoginResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Kindly enter all required fields",
                    };

                //  GET USER FROM DATABASE
                var hashedPassword = Util.StringHasher(loginRequest.Password);
                var getUserFromDb = await _repository.UserRepository.GetByEmailAndPassword(loginRequest.Email, hashedPassword, trackChanges);

                //  CHECK IF USER EXIST
                if (getUserFromDb == null)
                    return new GenericResponse<LoginResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "User not found",
                    };

                //  THIS IS THE RESPONSE DATA TO SEND BACK TO OUR CONSUMER
                var response = new LoginResponse()
                {
                    UserId = getUserFromDb.UserId,
                    Email = getUserFromDb.Email,
                    Name = getUserFromDb.Name,
                    AccessToken = getUserFromDb.AccessToken,
                    RefreshToken = getUserFromDb.RefreshToken,
                    CreatedAt = getUserFromDb.CreatedAt,
                    TokenGenerationTime = getUserFromDb.TokenGenerationTime
                };

                return new GenericResponse<LoginResponse>
                {
                    IsSuccessful = true,
                    ResponseCode = "200",
                    ResponseMessage = "User fetched successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<LoginResponse>
                {
                    IsSuccessful = false,
                    ResponseCode = "500",
                    ResponseMessage = "Error occured while fetching user",
                };
            }
        }

        public async Task<GenericResponse<Response>> UpdateUser(string userId, UpdateUserRequest request)
        {
            try
            {
                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if (string.IsNullOrEmpty(userId))
                    return new GenericResponse<Response>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Please, enter your User ID",
                    };

                var checkIfUserExist = await _repository.UserRepository.GetByUserId(userId, true);

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
                    ResponseMessage = "User updated successfully",
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Response>
                {
                    IsSuccessful = false,
                    ResponseCode = "400",
                    ResponseMessage = "Error occured while updating your user",
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
                        ResponseMessage = "Please, enter all required fields",
                    };

                //  CHECK IF USER EXIST IN DATABASE
                var getUserFromDb = await _repository.UserRepository.GetByUserId(userId, true);
                if (getUserFromDb == null)
                    return new GenericResponse<Response>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "User not exist",
                    };

                //  CHECK IF TASK EXIST IN DATABASE
                var checkIfTaskExistInUserTaskDb = await _repository.TaskRepository.GetTaskByTaskId(taskId, false);

                if (checkIfTaskExistInUserTaskDb == null)
                {
                    return new GenericResponse<Response>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "The task you just assigned does not exist",
                    };
                }
                else
                {
                    //  TRANSFER ALL CURRENT TASKS IN THE USER INTO A NEW VARIABLE SO IT WILL BE MANIPULATED EASILY
                    List<UserTask> getUserTasks = (List<UserTask>)await _repository.TaskRepository.GetTasksByUserId(userId, true);

                    //  THIS CONDITION WILL CHECK IF WE NEED TO ADD OR DELETE THE TASK
                    if (operation == AddOrDelete.Add)
                    {
                        //  CHECK IF THE TASK TO ADD ALREADY EXIST
                        if (Util.IsListContainTask(getUserTasks, checkIfTaskExistInUserTaskDb))
                        {
                            return new GenericResponse<Response>
                            {
                                IsSuccessful = false,
                                ResponseCode = "409",
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
                    ResponseMessage = "User updated successfully",
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Response>
                {
                    IsSuccessful = false,
                    ResponseCode = "500",
                    ResponseMessage = "Error occured while adding new task to this user",
                };
            }
        }

        public async Task<GenericResponse<TokenDto>> RefreshToken(RefreshTokenDto request)
        {
            try
            {
                var user = await _repository.UserRepository.GetByUserId(request.UserId, false);
                //check if user is null or not
                if (user == null)
                {
                    return new GenericResponse<TokenDto>
                    {
                        ResponseCode = "400",
                        ResponseMessage = "User is not logged in or does not exists",
                        IsSuccessful = false,
                    };
                }
                if (user.RefreshToken != request.RefreshToken)
                {
                    return new GenericResponse<TokenDto>
                    {
                        ResponseCode = "400",
                        ResponseMessage = "Refresh token not valid",
                        IsSuccessful = false,
                    };
                }

                var loginResponse = new GenericResponse<LoginResponse>
                {
                    ResponseCode = "200",
                    ResponseMessage = "Success",
                    Data = new LoginResponse
                    {
                        UserId = user.UserId,
                        Name = user.Name,
                        Email = user.Email,
                        CreatedAt = user.CreatedAt,
                    }
                };

                var tokenDto = new TokenDto
                {
                    AccessToken = _tokenManager.GenerateToken(ref loginResponse),
                    RefreshToken = _tokenManager.GenerateRefreshToken()
                };

                return new GenericResponse<TokenDto>
                {
                    ResponseCode = "200",
                    ResponseMessage = "Success",
                    IsSuccessful = true,
                    Data = tokenDto
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<TokenDto>
                {
                    ResponseCode = "500",
                    ResponseMessage = "Something went wrong while refreshing your token",
                    IsSuccessful = false,
                };
            }
        }

    }
}
