using Domain;
using Domain.Models;
using TaskManager.Application.Repository.Interfaces;
using TaskManager.Application.Service.Interfaces;
using TaskManager.Domain.Dtos;

namespace TaskManager.Infrastructure.Services
{
    internal sealed class TaskService : ITaskService
    {
        private readonly IRepositoryManager _repository;

        public TaskService(IRepositoryManager repositoryManager)
        {
            _repository = repositoryManager;
        }

        public async Task<GenericResponse<IEnumerable<TaskResponse>>> GetAllTasks()
        {
            try
            {                
                // THIS WILL GET ALL TASKS FROM THE REPOSITORY
                var allTasks = await _repository.TaskRepository.GetTasks();
                
                //  CHECK IF THE LIST IS EMPTY
                if (allTasks.Count() == 0)
                    return new GenericResponse<IEnumerable<TaskResponse>>
                    {
                        IsSuccessful = true,
                        ResponseCode = "200",
                        ResponseMessage = "No tasks found",
                        Data = null
                    };

                var response = new List<TaskResponse>();
                foreach (var task in allTasks)
                {
                    response.Add(new TaskResponse()
                    {
                        Id = task.TaskId.ToString(),
                        Title = task.Title,
                        Description = task.Description,
                        DueDate = task.DueDate.ToString(),
                        Priority = task.Priority.ToString(),
                        Status = task.Status.ToString()
                    });
                }
                
                return new GenericResponse<IEnumerable<TaskResponse>>
                {
                    IsSuccessful = true,
                    ResponseCode = "200",
                    ResponseMessage = "Successfully fetched all tasks. Total number: "+allTasks.Count(),
                    Data = response
                };
            } 
            catch (Exception ex)
            {
                return new GenericResponse<IEnumerable<TaskResponse>>
                {
                    IsSuccessful = false,
                    ResponseCode = "400",
                    ResponseMessage = "Error occured while getting Tasks",
                    Data = null
                };
            }
        }
        public async Task<GenericResponse<TaskResponse>> GetTaskByTaskId(string taskIdString)
        {
            try
            {
                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if (string.IsNullOrEmpty(taskIdString))
                    return new GenericResponse<TaskResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Kindly enter your task Id in the query string",
                        Data = null
                    };

                // THIS WILL GET ALL TASKS FROM THE REPOSITORY
                Guid taskIdGuid = new Guid(taskIdString);
                var responseFromDb = await _repository.TaskRepository.GetTaskByTaskId(taskIdGuid, false);

                //  CHECK IF TASK EXIST
                if (responseFromDb == null)
                    return new GenericResponse<TaskResponse>
                    {
                        IsSuccessful = true,
                        ResponseCode = "200",
                        ResponseMessage = "Task not found",
                        Data = null
                    };

                var response = new TaskResponse()
                {
                    Id = responseFromDb.TaskId.ToString(),
                    Title = responseFromDb.Title,
                    Description = responseFromDb.Description,
                    DueDate = responseFromDb.DueDate.ToString(),
                    Priority = responseFromDb.Priority.ToString(),
                    Status = responseFromDb.Status.ToString()
                };

                return new GenericResponse<TaskResponse>
                {
                    IsSuccessful = true,
                    ResponseCode = "200",
                    ResponseMessage = "Successfully fetched task",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<TaskResponse>
                {
                    IsSuccessful = false,
                    ResponseCode = "400",
                    ResponseMessage = "Error occured while getting your Task",
                    Data = null
                };
            }
        }
        public async Task<GenericResponse<IEnumerable<TaskResponse>>> GetTasksDueThisWeek()
        {
            try
            {
                var getLastDateTimeOfCurrentWeek = Util.GetLastDayOfCurrentWeek();
                var todayDate = DateOnly.FromDateTime(DateTime.UtcNow);
                var responseFromDb = await _repository.TaskRepository.GetTasksDueThisWeek(todayDate, getLastDateTimeOfCurrentWeek, false);

                //  CHECK IF TASK EXIST
                if (responseFromDb.Count() == 0)
                    return new GenericResponse<IEnumerable<TaskResponse>>
                    {
                        IsSuccessful = true,
                        ResponseCode = "200",
                        ResponseMessage = "No Task found",
                        Data = null
                    };

                var response = new List<TaskResponse>();
                foreach (var task in responseFromDb)
                {
                    response.Add(new TaskResponse()
                    {
                        Id = task.TaskId.ToString(),
                        Title = task.Title,
                        Description = task.Description,
                        DueDate = task.DueDate.ToString(),
                        Priority = task.Priority.ToString(),
                        Status = task.Status.ToString()
                    });
                }
                return new GenericResponse<IEnumerable<TaskResponse>>
                {
                    IsSuccessful = true,
                    ResponseCode = "200",
                    ResponseMessage = "Successfully fetched task",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<IEnumerable<TaskResponse>>
                {
                    IsSuccessful = false,
                    ResponseCode = "400",
                    ResponseMessage = "Error occured while getting your Task",
                    Data = null
                };
            }
        }
        public async Task<GenericResponse<IEnumerable<TaskResponse>>> GetTaskByPriorityOrStatus(StatusAndPriorityRequest request)
        {
            try
            {
                // THIS WILL GET ALL TASKS FROM THE REPOSITORY
                var responseFromDb = await _repository.TaskRepository.GetTasksByStatusOrPriority(request.TaskStatus, request.TaskPriority, false);

                //  CHECK IF THE LIST IS EMPTY
                if (responseFromDb.Count() == 0)
                    return new GenericResponse<IEnumerable<TaskResponse>>
                    {
                        IsSuccessful = true,
                        ResponseCode = "200",
                        ResponseMessage = "No tasks found",
                        Data = null
                    };

                //  CHECK IF THE STATUS IS PART OF WHAT WE HAVE IN OUR ENUM
                if (!Enum.IsDefined(typeof(Status), request.TaskStatus))
                    return new GenericResponse<IEnumerable<TaskResponse>>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Sorry, you enterred a wrong status",
                        Data = null
                    };
                //  CHECK IF THE PRIORITY IS PART OF WHAT WE HAVE IN OUR ENUM
                if (!Enum.IsDefined(typeof(Priority), request.TaskPriority))
                    return new GenericResponse<IEnumerable<TaskResponse>>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Sorry, you enterred a wrong priority",
                        Data = null
                    };

                var response = new List<TaskResponse>();
                foreach (var task in responseFromDb)
                {
                    response.Add(new TaskResponse()
                    {
                        Id = task.TaskId.ToString(),
                        Title = task.Title,
                        Description = task.Description,
                        DueDate = task.DueDate.ToString(),
                        Priority = task.Priority.ToString(),
                        Status = task.Status.ToString()
                    });
                }

                return new GenericResponse<IEnumerable<TaskResponse>>
                {
                    IsSuccessful = true,
                    ResponseCode = "200",
                    ResponseMessage = "Successfully fetched all tasks. Total number: " + responseFromDb.Count(),
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<IEnumerable<TaskResponse>>
                {
                    IsSuccessful = false,
                    ResponseCode = "400",
                    ResponseMessage = "Error occured while getting Tasks",
                    Data = null
                };
            }
        }
        public async Task<GenericResponse<TaskResponse>> CreateTask(CreateTaskRequest task)
        {
            try
            {
                DateOnly dueDateInDateFormat;
                string dueDateInStringFormat = task.DueDate.ToString();
                
                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if(string.IsNullOrEmpty(task.Title) || string.IsNullOrEmpty(task.Description) || string.IsNullOrEmpty(dueDateInStringFormat))
                    return new GenericResponse<TaskResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Please, enter all fields",
                        Data = null
                    };

                //  CHECK IF THE PRIORITY IS PART OF WHAT WE HAVE IN OUR ENUM.......... AND ENSURE USER CAN NOT SELECT NONE WHILE CREATING
                if (!Enum.IsDefined(typeof(Priority), task.Priority) || Priority.None == task.Priority)
                    return new GenericResponse<TaskResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Sorry, you enterred a wrong priority",
                        Data = null
                    };

                //  TRY TO CONVERT THE DATETIME
                if (!DateOnly.TryParse(dueDateInStringFormat, out dueDateInDateFormat))
                {
                    return new GenericResponse<TaskResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Please, enter a correct date and time format",
                        Data = null
                    };
                }

                //  PREVENT A USER FROM SELECTING A PAST DATE AS DUE DATE
                if (dueDateInDateFormat < DateOnly.FromDateTime(DateTime.UtcNow))
                {
                    return new GenericResponse<TaskResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Sorry, you can't select a time from the past",
                        Data = null
                    };
                }

                var taskId = Guid.NewGuid();

                //  SEND EMAIL TO USER
                Util.SendEmail("kellylambeth93@gmail.com", NotificationType.Status_update.ToString(), "A new task with id: " + taskId.ToString() + " has been assigned to you at time: "+ DateTime.UtcNow.ToString());
                

                UserTask taskToSave = new UserTask
                {
                    TaskId = taskId,
                    Title = task.Title,
                    Description = task.Description,
                    DueDate = dueDateInDateFormat,
                    Priority = task.Priority,
                    Status = Status.Pending,
                };
                _repository.TaskRepository.CreateTask(taskToSave);
                await _repository.SaveAsync();

                //  CHECK IF THE LIST IS EMPTY
                return new GenericResponse<TaskResponse>
                {
                    IsSuccessful = true,
                    ResponseCode = "201",
                    ResponseMessage = "You just successfully created a new task",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<TaskResponse>
                {
                    IsSuccessful = false,
                    ResponseCode = "400",
                    ResponseMessage = "Error occured while creating your new Task",
                    Data = null
                };
            }
        }
        public async Task<GenericResponse<TaskResponse>> UpdateTask(string taskIdString, StatusAndPriorityRequest request)
        {
            try
            {
                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if (string.IsNullOrEmpty(taskIdString))
                    return new GenericResponse<TaskResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Kindly enter your task Id in the query string",
                        Data = null
                    };

                //  CHECK IF THE STATUS IS PART OF WHAT WE HAVE IN OUR ENUM
                if (!Enum.IsDefined(typeof(Status), request.TaskStatus))
                    return new GenericResponse<TaskResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Sorry, you enterred a wrong status",
                        Data = null
                    };
                //  CHECK IF THE PRIORITY IS PART OF WHAT WE HAVE IN OUR ENUM
                if (!Enum.IsDefined(typeof(Priority), request.TaskPriority))
                    return new GenericResponse<TaskResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Sorry, you enterred a wrong priority",
                        Data = null
                    };

                Guid taskIdGuid = new Guid(taskIdString);
                var checkIfTaskExist = await _repository.TaskRepository.GetTaskByTaskId(taskIdGuid, true);
                
                //  CHECK IF THE TASK EXIST
                if (checkIfTaskExist == null)
                    return new GenericResponse<TaskResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Task not found",
                    };

                //  PREVENT USER FROM UPDATING AN OUTDATED TASK
                DateOnly dueDate = (DateOnly)checkIfTaskExist.DueDate;
                if (dueDate < DateOnly.FromDateTime(DateTime.UtcNow))
                {
                    return new GenericResponse<TaskResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Sorry, this task has already expired and can not be edited anymore",
                    };
                }

                //  CONFIRM IF THE TASK CURRENT STATUS IS NOT COMPLETED 
                if (checkIfTaskExist.Status != Status.completed)
                {
                    //  THEN CONFIRM IF THE NEW STATUS IS COMPLETED SO THAT A NOTIFICATION WILL BE SENT OUT
                    if (request.TaskStatus == Status.completed)
                    {
                        Util.SendEmail("kellylambeth93@gmail.com",NotificationType.Status_update.ToString(), "The task with id: " + checkIfTaskExist.TaskId + " was just completed  at Time: "+ DateTime.UtcNow.ToString());
                    }
                }

                //  THIS CONDITION WILL CHECK IF 0 (i.e 'None') IS ASSIGNED TO EITHER THE STATUS OR THE PRIORITY FIELD SO WE WONT HAVE TO CHANGE THE VALUE
                checkIfTaskExist.Status = request.TaskStatus == Status.None ? checkIfTaskExist.Status : request.TaskStatus;
                checkIfTaskExist.Priority = request.TaskPriority == Priority.None ? checkIfTaskExist.Priority : request.TaskPriority;
                _repository.TaskRepository.UpdateTask(checkIfTaskExist);
                await _repository.SaveAsync();



                return new GenericResponse<TaskResponse>
                {
                    IsSuccessful = true,
                    ResponseCode = "200",
                    ResponseMessage = "Successfully updated your task in the database",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<TaskResponse>
                {
                    IsSuccessful = false,
                    ResponseCode = "400",
                    ResponseMessage = "Error occured while saving Tasks",
                    Data = null
                };
            }
        }
        public async Task<GenericResponse<TaskResponse>> DeleteTask(string taskIdString)
        {
            try
            {
                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if (string.IsNullOrEmpty(taskIdString))
                    return new GenericResponse<TaskResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Please, enter the task Id",
                        Data = null
                    };
                
                Guid taskIdGuid = new Guid(taskIdString);
                var checkIfTaskExist = await _repository.TaskRepository.GetTaskByTaskId(taskIdGuid, true);
                
                //  CHECK IF THE TASK EXIST
                if (checkIfTaskExist == null)
                    return new GenericResponse<TaskResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Task not found",
                        Data = null
                    };

                _repository.TaskRepository.DeleteTask(checkIfTaskExist);
                await _repository.SaveAsync();

                return new GenericResponse<TaskResponse>
                {
                    IsSuccessful = true,
                    ResponseCode = "200",
                    ResponseMessage = "Successfully deleted your task in the database",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<TaskResponse>
                {
                    IsSuccessful = false,
                    ResponseCode = "400",
                    ResponseMessage = "Error occured while saving Tasks",
                    Data = null
                };
            }
        }

    }
}