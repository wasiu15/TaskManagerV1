﻿using Domain;
using TaskManager.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using TaskManager.Application.Repository.Interfaces;
using TaskManager.Application.Service.Interfaces;
using TaskManager.Domain.Dtos;
using TaskManager.Infrastructure.Utilities;
using Hangfire;

namespace TaskManager.Infrastructure.Services
{
    internal sealed class TaskService : ITaskService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientWrapper _httpClient;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IRepositoryManager _repository;

        public TaskService(IRepositoryManager repositoryManager, IHttpContextAccessor httpContext, IConfiguration configuration, IHttpClientWrapper httpClient)
        {
            _repository = repositoryManager;
            _httpContext = httpContext;
            _configuration = configuration;
            _httpClient = httpClient;

            //  CALL THE SHEDULLER
            //StartSchedulerJob();
        }

        public async Task<GenericResponse<IEnumerable<TaskResponse>>> GetAllTasks()
        {
            try
            {
                SendReminderForDueTasks();
                // THIS WILL GET ALL TASKS FROM THE REPOSITORY
                var allTasks = await _repository.TaskRepository.GetTasks();

                //  CHECK IF THE LIST IS EMPTY
                if (allTasks == null)
                    return new GenericResponse<IEnumerable<TaskResponse>>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Tasks not found",
                    };

                var response = new List<TaskResponse>();
                foreach (var task in allTasks)
                {
                    response.Add(new TaskResponse()
                    {
                        Id = task.Id.ToString(),
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
                    ResponseMessage = "Successfully fetched all tasks. Total number: " + allTasks.Count(),
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<IEnumerable<TaskResponse>>
                {
                    IsSuccessful = false,
                    ResponseCode = "500",
                    ResponseMessage = "Error occured while fetching Tasks"
                };
            }
        }
        public async Task<GenericResponse<IEnumerable<TaskResponse>>> GetAllTasksByUserId()
        {
            try
            {
                var currentUserId = _httpContext.HttpContext?.GetSessionUser().UserId ?? "";
                // THIS WILL GET ALL TASKS FROM THE REPOSITORY
                var allTasks = await _repository.TaskRepository.GetTasksByUserId(currentUserId, false);

                //  CHECK IF THE LIST IS EMPTY
                if (allTasks == null)
                    return new GenericResponse<IEnumerable<TaskResponse>>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Tasks not found for this user",
                    };

                var response = new List<TaskResponse>();
                foreach (var task in allTasks)
                {
                    response.Add(new TaskResponse()
                    {
                        Id = task.Id.ToString(),
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
                    ResponseMessage = "Successfully fetched all tasks. Total number: " + allTasks.Count(),
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<IEnumerable<TaskResponse>>
                {
                    IsSuccessful = false,
                    ResponseCode = "500",
                    ResponseMessage = "Error occured while fetching Tasks"
                };
            }
        }
        public async Task<GenericResponse<TaskResponse>> GetTaskByTaskId(string taskId)
        {
            try
            {
                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if (string.IsNullOrEmpty(taskId))
                    return new GenericResponse<TaskResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Kindly enter your Task ID"
                    };

                // THIS WILL GET ALL TASKS FROM THE REPOSITORY
                Guid taskIdGuid = new Guid(taskId);
                var responseFromDb = await _repository.TaskRepository.GetTaskByTaskId(taskId, false);

                //  CHECK IF TASK EXIST
                if (responseFromDb == null)
                    return new GenericResponse<TaskResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Task not found",
                    };

                var response = new TaskResponse()
                {
                    Id = responseFromDb.Id.ToString(),
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
                    ResponseMessage = "Task fetched Successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<TaskResponse>
                {
                    IsSuccessful = false,
                    ResponseCode = "500",
                    ResponseMessage = "Error occured while fetching your Task",
                };
            }
        }
        public async Task<GenericResponse<IEnumerable<TaskResponse>>> GetTasksDueThisWeek()
        {
            try
            {
                var getLastDateTimeOfCurrentWeek = Util.GetLastDayOfCurrentWeek();
                var todayDate = DateTime.UtcNow;
                var responseFromDb = await _repository.TaskRepository.GetTasksDueThisWeek(todayDate, getLastDateTimeOfCurrentWeek, false);

                //  CHECK IF TASK EXIST
                if (responseFromDb.Count() == 0)
                    return new GenericResponse<IEnumerable<TaskResponse>>
                    {
                        IsSuccessful = true,
                        ResponseCode = "200",
                        ResponseMessage = "No Task found",
                    };

                var response = new List<TaskResponse>();
                foreach (var task in responseFromDb)
                {
                    response.Add(new TaskResponse()
                    {
                        Id = task.Id.ToString(),
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
                    ResponseMessage = "Successfully fetched task total number is: " + response.Count(),
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<IEnumerable<TaskResponse>>
                {
                    IsSuccessful = false,
                    ResponseCode = "500",
                    ResponseMessage = "Error occured while fetching your Task",
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
                if (responseFromDb == null)
                    return new GenericResponse<IEnumerable<TaskResponse>>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Tasks not found",
                    };

                //  CHECK IF THE STATUS IS PART OF WHAT WE HAVE IN OUR ENUM
                if (!Enum.IsDefined(typeof(Status), request.TaskStatus))
                    return new GenericResponse<IEnumerable<TaskResponse>>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Sorry, you enterred a wrong status",
                    };
                //  CHECK IF THE PRIORITY IS PART OF WHAT WE HAVE IN OUR ENUM
                if (!Enum.IsDefined(typeof(Priority), request.TaskPriority))
                    return new GenericResponse<IEnumerable<TaskResponse>>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Sorry, you enterred a wrong priority",
                    };


                var response = new List<TaskResponse>();
                foreach (var task in responseFromDb)
                {
                    response.Add(new TaskResponse()
                    {
                        Id = task.Id.ToString(),
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
                    ResponseCode = "500",
                    ResponseMessage = "Error occured while getting Tasks",
                };
            }
        }
        public async Task<GenericResponse<TaskResponse>> CreateTask(CreateTaskRequest task)
        {
            try
            {
                DateTime dueDateInDateFormat;
                string dueDateInStringFormat = task.DueDate.ToString();

                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if (string.IsNullOrEmpty(task.Title) || string.IsNullOrEmpty(task.Description) || string.IsNullOrEmpty(dueDateInStringFormat))
                    return new GenericResponse<TaskResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Please, enter all required fields",
                    };

                //  CHECK IF THE PRIORITY IS PART OF WHAT WE HAVE IN OUR ENUM.......... AND ENSURE USER CAN NOT SELECT NONE WHILE CREATING
                if (!Enum.IsDefined(typeof(Priority), task.Priority) || Priority.None == task.Priority)
                    return new GenericResponse<TaskResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Sorry, you enterred a wrong priority",
                    };

                //  TRY TO CONVERT THE DATETIME
                if (!DateTime.TryParse(dueDateInStringFormat, out dueDateInDateFormat))
                {
                    return new GenericResponse<TaskResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Please, enter a correct date and time format",
                    };
                }

                //  PREVENT A USER FROM SELECTING A PAST DATE AS DUE DATE
                if (dueDateInDateFormat < DateTime.UtcNow)
                {
                    return new GenericResponse<TaskResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Sorry, you can't select a time from the past",
                    };
                }

                //  GET CURRENT USER EMAIL AND NAME FROM THE HTTPCONTEXT CLASS
                var currentUserEmail = _httpContext.HttpContext?.GetSessionUser().Email ?? "";
                var currentUserName = _httpContext.HttpContext?.GetSessionUser().Name ?? "";
                var sendRequest = new EmailSenderRequestDto
                {
                    email = currentUserEmail,
                    subject = "Message From Task Manager",
                    message = "Hello " + currentUserName + ", You just created a new task with the Title: " + task.Title + " at Time: " + DateTime.UtcNow.ToString()
                };

                //  CALL THE BACKGROUND WORKER
                ScheduleTaskEmail(sendRequest);
                //  CALLED THE BACKGROUND WORKER


                var currentUserId = _httpContext.HttpContext?.GetSessionUser().UserId ?? "";

                //  SAVE TO TASK TABLE
                UserTask taskToSave = new UserTask
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = task.Title,
                    Description = task.Description,
                    DueDate = dueDateInDateFormat,
                    Priority = task.Priority,
                    IsReminderSent = false,
                    UserId = currentUserId,
                    Status = Status.Pending,
                };
                _repository.TaskRepository.CreateTask(taskToSave);


                //  SAVE TO NOTIFICATION TABLE
                Notification notificationToSave = new Notification
                {
                    NotificationId = Guid.NewGuid().ToString(),
                    TaskId = taskToSave.Id,
                    RecievedUserId = currentUserId,
                    Type = NotificationType.Status_update.ToString(),
                    Message = sendRequest.message,
                    ReadStatus = NotificationStatus.Unread.ToString(),
                    Time = DateTime.UtcNow,
                };
                _repository.NotificationRepository.CreateNotification(notificationToSave);


                await _repository.SaveAsync();

                //  CREATE RESPONSE TO SEND OUT
                var response = new TaskResponse()
                {
                    Id = taskToSave.Id.ToString(),
                    Title = taskToSave.Title,
                    Description = taskToSave.Description,
                    DueDate = taskToSave.DueDate.ToString(),
                    Priority = taskToSave.Priority.ToString(),
                    Status = taskToSave.Status.ToString()
                };

                //  CHECK IF THE LIST IS EMPTY
                return new GenericResponse<TaskResponse>
                {
                    IsSuccessful = true,
                    ResponseCode = "201",
                    ResponseMessage = "New task created successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<TaskResponse>
                {
                    IsSuccessful = false,
                    ResponseCode = "500",
                    ResponseMessage = "Error occured while creating your new Task",
                };
            }
        }
        public async Task<GenericResponse<TaskResponse>> UpdateTask(string taskId, StatusAndPriorityRequest request)
        {
            try
            {
                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if (string.IsNullOrEmpty(taskId))
                    return new GenericResponse<TaskResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Kindly enter your Task ID in the query string",
                    };

                //  CHECK IF THE STATUS IS PART OF WHAT WE HAVE IN OUR ENUM
                if (!Enum.IsDefined(typeof(Status), request.TaskStatus))
                    return new GenericResponse<TaskResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Sorry, you enterred a wrong status",
                    };
                //  CHECK IF THE PRIORITY IS PART OF WHAT WE HAVE IN OUR ENUM
                if (!Enum.IsDefined(typeof(Priority), request.TaskPriority))
                    return new GenericResponse<TaskResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Sorry, you enterred a wrong priority",
                    };

                var checkIfTaskExist = await _repository.TaskRepository.GetTaskByTaskId(taskId, true);

                //  CHECK IF THE TASK EXIST
                if (checkIfTaskExist == null)
                    return new GenericResponse<TaskResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Task not found",
                    };

                //  PREVENT USER FROM UPDATING AN OUTDATED TASK
                DateTime dueDate = checkIfTaskExist.DueDate;
                if (dueDate < DateTime.UtcNow)
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
                        //  GET CURRENT USER EMAIL AND NAME FROM THE HTTPCONTEXT CLASS
                        var currentUserEmail = _httpContext.HttpContext?.GetSessionUser().Email ?? "";
                        var currentUserName = _httpContext.HttpContext?.GetSessionUser().Name ?? "";
                        var sendRequest = new EmailSenderRequestDto
                        {
                            email = currentUserEmail,
                            subject = "Message From Task Manager",
                            message = "Hello " + currentUserName + ", The task with Title: " + checkIfTaskExist.Title + " was just completed at Time: " + DateTime.UtcNow.ToString()
                        };

                        //  CALL THE BACKGROUND WORKER
                        ScheduleTaskEmail(sendRequest);
                        //  CALLED THE BACKGROUND WORKER


                        var currentUserId = _httpContext.HttpContext?.GetSessionUser().UserId ?? "";
                        Notification notificationToSave = new Notification
                        {
                            NotificationId = Guid.NewGuid().ToString(),
                            TaskId = checkIfTaskExist.Id,
                            RecievedUserId = currentUserId,
                            Message = sendRequest.message,
                            Type = NotificationType.Status_update.ToString(),
                            ReadStatus = NotificationStatus.Unread.ToString(),
                            Time = DateTime.UtcNow
                        };
                        _repository.NotificationRepository.CreateNotification(notificationToSave);
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
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<TaskResponse>
                {
                    IsSuccessful = false,
                    ResponseCode = "400",
                    ResponseMessage = "Error occured while saving Tasks",
                };
            }
        }
        public async Task<GenericResponse<TaskResponse>> DeleteTask(string taskId)
        {
            try
            {
                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if (string.IsNullOrEmpty(taskId))
                    return new GenericResponse<TaskResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Please, enter the task Id",
                    };

                Guid taskIdGuid = new Guid(taskId);
                var checkIfTaskExist = await _repository.TaskRepository.GetTaskByTaskId(taskId, true);

                //  CHECK IF THE TASK EXIST
                if (checkIfTaskExist == null)
                    return new GenericResponse<TaskResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Task not found",
                    };

                //  CHECK IF THIS TASK IS RELATED TO ANY PROJECT
                var listOfRelatedProjectUserTasks = await _repository.ProjectTaskRepository.GetByTaskId(taskId, true);
                //  DELETE FOREIGN KEYS IF FOUND
                if (listOfRelatedProjectUserTasks != null)
                {
                    //  DELETE FOREIGN KEY(S) FIRST
                    _repository.ProjectTaskRepository.DeleteProjectUserTasks(listOfRelatedProjectUserTasks);
                }

                //  DELETE FROM TASK DATABASE
                _repository.TaskRepository.DeleteTask(checkIfTaskExist);
                await _repository.SaveAsync();

                return new GenericResponse<TaskResponse>
                {
                    IsSuccessful = true,
                    ResponseCode = "200",
                    ResponseMessage = "Successfully deleted your task in the database",
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<TaskResponse>
                {
                    IsSuccessful = false,
                    ResponseCode = "400",
                    ResponseMessage = "Error occured while saving Tasks",
                };
            }
        }



        // BACKGROUND JOB METHOD
        public async Task ScheduleTaskEmail(EmailSenderRequestDto messageDetails)
        {
            // Schedule a background job to send the email
            BackgroundJob.Enqueue(() => SendEmailAsync(messageDetails));
        }

        public async Task SendEmailAsync(EmailSenderRequestDto sendRequest)
        {
            string mailerUrl = _configuration.GetSection("ExternalAPIs")["MailerUrl"];

            // Send the request to the email server
            await _httpClient.SendPostEmailAsync<string>(mailerUrl, sendRequest);
        }

        public void StartSchedulerJob()
        {
            // Remove any existing job with the same name
            RecurringJob.RemoveIfExists("dueDateJob");

            // Trigger the job immediately and schedule it to run every 5 minutes
            RecurringJob.AddOrUpdate("dueDateJob", () => SendReminderForDueTasks(), Cron.MinuteInterval(2));
        }


        //  METHOD TO FIND DUE TASKS
        public async Task SendReminderForDueTasks()
        {
            try
            {
                var dueTasks = await _repository.TaskRepository.GetAnyUncompletedTaskToDueInTwoDaysWithNotificationStatusOffFalse(false);
                if (dueTasks != null)
                {
                    foreach (var task in dueTasks)
                    {
                        var user = await _repository.UserRepository.GetByUserId(task.UserId, false);

                        if (user != null)
                        {
                            var sendRequest = new EmailSenderRequestDto
                            {
                                email = user.Email,
                                subject = "Due Date Reminder",
                                message = $"Hello {user.Name}, Your task with the Title: {task.Title} now has less than 48 hours left to be completed as of {DateTime.UtcNow.ToString()}"
                            };
                            Console.WriteLine(sendRequest.email);

                            // Send mail to the user
                            SendEmailAsync(sendRequest);

                            var createNotificationRequest = new Notification
                            {
                                NotificationId = Guid.NewGuid().ToString(),
                                TaskId = task.Id.ToString(),
                                RecievedUserId = task.UserId,
                                Type = NotificationType.Due_date.ToString(),
                                Message = sendRequest.message,
                                ReadStatus = NotificationStatus.Unread.ToString(),
                                Time = DateTime.UtcNow,
                            };

                            task.IsReminderSent = true;
                            _repository.TaskRepository.UpdateTask(task);
                            _repository.NotificationRepository.CreateNotification(createNotificationRequest);
                        }
                    }

                    // Save changes to the repository
                    await _repository.SaveAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }



    }
}