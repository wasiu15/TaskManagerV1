using Domain;
using TaskManager.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using TaskManager.Application.Repository.Interfaces;
using TaskManager.Application.Service.Interfaces;
using TaskManager.Domain.Dtos;
using TaskManager.Infrastructure.Utilities;

namespace TaskManager.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientWrapper _httpClient;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IRepositoryManager _repository;

        public NotificationService(IRepositoryManager repository, IHttpContextAccessor httpContext, IConfiguration configuration, IHttpClientWrapper httpClient)
        {
            _repository = repository;
            _httpContext = httpContext;
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<GenericResponse<Response>> CreateNotification(CreateNotificationRequest notification)
        {
            try
            {
                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if (string.IsNullOrEmpty(notification.TaskId) || string.IsNullOrEmpty(notification.Message))
                    return new GenericResponse<Response>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Please, enter all fields",
                    };
                
                //  CHECK IF THE NOTIFICATION TYPE ENTERED IS ONE OF OUR CUSTOM NOTIFICATION TYPE
                if (notification.Type != NotificationType.Status_update && notification.Type != NotificationType.Due_date)
                    return new GenericResponse<Response>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Please, enter a valid notification type",
                    };
                
                //  CHECK IF TASK EXIST IN DATABASE
                var checkIfTaskExist = await _repository.TaskRepository.GetTaskByTaskId(notification.TaskId, false);
                if (checkIfTaskExist == null)
                    return new GenericResponse<Response>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Task not found",
                    };

                //  SEND NOTIFICATION 

                //  GET CURRENT USER EMAIL AND NAME FROM THE HTTPCONTEXT CLASS
                var currentUserEmail = _httpContext.HttpContext?.GetSessionUser().Email ?? "";
                var sendRequest = new EmailSenderRequestDto
                {
                    email = currentUserEmail,
                    subject = notification.Type.ToString().Replace('_',' '),
                    message = notification.Message
                };

                //  GET THE MAILER URL... WHERE WE WOULD BE SENDING OUR POST REQUEST TO
                var mailerUrl = $"{_configuration.GetSection("ExternalAPIs")["MailerUrl"]}";

                //  THIS LINE SENDS THE REQUEST TO THE EMAIL SERVER
                var sendEmailResponse = _httpClient.SendPostEmailAsync<string>(mailerUrl, sendRequest);

                //  WE ARE NOT CHECKING IF IT WAS SUCCESSFUL OR NOT HERE BECAUSE EVEN IT THE EMAIL SERVER FAILS
                //  THE TASK PROCESS SHOULD CONTINUE (BASE OF THIS APPLICATION REQUIREMENT WE DONT WANT TO MAKE THINGS TOO COMPLICATED)

                var currentUserId = _httpContext.HttpContext?.GetSessionUser().UserId ?? "";
                Notification notificationToSave = new Notification
                {
                    NotificationId = Guid.NewGuid().ToString(),
                    TaskId = notification.TaskId,
                    RecievedUserId = currentUserId,
                    Message = notification.Message,
                    Type = notification.Type == NotificationType.Due_date ? NotificationType.Due_date.ToString() : NotificationType.Status_update.ToString(),
                    ReadStatus = NotificationStatus.Unread.ToString(),
                    Time = DateTime.UtcNow
                };
                _repository.NotificationRepository.CreateNotification(notificationToSave);
                await _repository.SaveAsync();

                return new GenericResponse<Response>
                {
                    IsSuccessful = true,
                    ResponseCode = "201",
                    ResponseMessage = "Notification created successfully",
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Response>
                {
                    IsSuccessful = false,
                    ResponseCode = "500",
                    ResponseMessage = "Error occured while creating your new notification",
                };
            }
        }

        public async Task<GenericResponse<Response>> DeleteNotification(string notificationId)
        {
            try
            {
                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if (string.IsNullOrEmpty(notificationId))
                    return new GenericResponse<Response>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Please, enter the notification Id",
                    };

                var checkIfNotificationExist = await _repository.NotificationRepository.GetByNotificationId(notificationId, true);

                //  CHECK IF THE NOTIFICATION EXIST
                if (checkIfNotificationExist == null)
                    return new GenericResponse<Response>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Notification not found",
                    };

                _repository.NotificationRepository.DeleteNotification(checkIfNotificationExist);
                await _repository.SaveAsync();

                return new GenericResponse<Response>
                {
                    IsSuccessful = true,
                    ResponseCode = "200",
                    ResponseMessage = "Notification deleted Successfully",
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Response>
                {
                    IsSuccessful = false,
                    ResponseCode = "500",
                    ResponseMessage = "Error occured while deleting your notification",
                };
            }
        }

        public async Task<GenericResponse<IEnumerable<Notification>>> GetAllNotifications()
        {
            try
            {
                // THIS WILL GET ALL TASKS FROM THE REPOSITORY
                var allNotifications = await _repository.NotificationRepository.GetNotifications();

                //  CHECK IF THE LIST IS EMPTY
                if (allNotifications.Count() == 0)
                    return new GenericResponse<IEnumerable<Notification>>
                    {
                        IsSuccessful = true,
                        ResponseCode = "200",
                        ResponseMessage = "Notification not found",
                    };

                return new GenericResponse<IEnumerable<Notification>>
                {
                    IsSuccessful = true,
                    ResponseCode = "200",
                    ResponseMessage = "Successfully fetched all notifications. Total number: " + allNotifications.Count(),
                    Data = allNotifications
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<IEnumerable<Notification>>
                {
                    IsSuccessful = false,
                    ResponseCode = "500",
                    ResponseMessage = "Error occured while getting notifications",
                };
            }
        }

        public async Task<GenericResponse<NotificationDto>> GetByNotificationId(string notificationId)
        {
            try
            {
                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if (string.IsNullOrEmpty(notificationId))
                    return new GenericResponse<NotificationDto>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Kindly enter your notification Id in the query string",
                    };

                // THIS WILL GET ALL NOTIFICATION FROM THE REPOSITORY
                var getNotificationFromDb = await _repository.NotificationRepository.GetByNotificationId(notificationId, false);

                //  CHECK IF USER EXIST
                if (getNotificationFromDb == null)
                    return new GenericResponse<NotificationDto>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Notification not found",
                    };

                //  THIS IS THE RESPONSE DATA TO SEND BACK TO OUR CONSUMER
                var response = new NotificationDto()
                {
                    Type = getNotificationFromDb.Type.ToString(),
                    TaskId = getNotificationFromDb.TaskId,
                    Message = getNotificationFromDb.Message,
                    ReadStatus = NotificationStatus.Unread.ToString(),
                    Time = getNotificationFromDb.Time,
                };

                return new GenericResponse<NotificationDto>
                {
                    IsSuccessful = true,
                    ResponseCode = "200",
                    ResponseMessage = "Notifications fetched Successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<NotificationDto>
                {
                    IsSuccessful = false,
                    ResponseCode = "500",
                    ResponseMessage = "Error occured while getting your notifcation",
                };
            }
        }

        public async Task<GenericResponse<Response>> ReadOrUnread(string notificationId)
        {
            try
            {
                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if (string.IsNullOrEmpty(notificationId))
                    return new GenericResponse<Response>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Kindly enter your Notification ID in the query string",
                    };

                var checkIfNotificationExist = await _repository.NotificationRepository.GetByNotificationId(notificationId, true);

                //  CHECK IF THE NOTIFICATION EXIST
                if (checkIfNotificationExist == null)
                    return new GenericResponse<Response>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Notification not found",
                    };

                checkIfNotificationExist.ReadStatus = checkIfNotificationExist.ReadStatus == "Read" ? "Unread" : "Read";
                _repository.NotificationRepository.UpdateNotification(checkIfNotificationExist);
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
                    ResponseCode = "500",
                    ResponseMessage = "Error occured while updating your notification",
                };
            }
        }
    }
}
