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
    public class NotificationService : INotificationService
    {
        private readonly IRepositoryManager _repository;

        public NotificationService(IRepositoryManager repository)
        {
            this._repository = repository;
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
                Guid convertTaskIdToGuid = new Guid(notification.TaskId);
                var checkIfTaskExist = await _repository.TaskRepository.GetTaskByTaskId(convertTaskIdToGuid, false);
                if (checkIfTaskExist == null)
                    return new GenericResponse<Response>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Task not found",
                    };

                Notification notificationToSave = new Notification
                {
                    NotificationId = Guid.NewGuid(),
                    TaskId = notification.TaskId,
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
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Response>
                {
                    IsSuccessful = false,
                    ResponseCode = "400",
                    ResponseMessage = "Error occured while creating your new notification",
                    Data = null
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

                Guid notificationGuid = new Guid(notificationId);
                var checkIfNotificationExist = await _repository.NotificationRepository.GetByNotificationId(notificationGuid, true);

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
                    ResponseCode = "400",
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
                    ResponseCode = "400",
                    ResponseMessage = "Error occured while getting notifications",
                };
            }
        }

        public async Task<GenericResponse<NotificationDto>> GetByNotificationId(string notificationIdString)
        {
            try
            {
                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if (string.IsNullOrEmpty(notificationIdString))
                    return new GenericResponse<NotificationDto>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Kindly enter your notification Id in the query string",
                    };

                // THIS WILL GET ALL NOTIFICATION FROM THE REPOSITORY
                Guid notificationIdGuid = new Guid(notificationIdString);
                var getNotificationFromDb = await _repository.NotificationRepository.GetByNotificationId(notificationIdGuid, false);

                //  CHECK IF USER EXIST
                if (getNotificationFromDb == null)
                    return new GenericResponse<NotificationDto>
                    {
                        IsSuccessful = true,
                        ResponseCode = "200",
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
                    ResponseCode = "400",
                    ResponseMessage = "Error occured while getting your notifcation",
                };
            }
        }

        public async Task<GenericResponse<Response>> ReadOrUnread(string notificationIdString)
        {
            try
            {
                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if (string.IsNullOrEmpty(notificationIdString))
                    return new GenericResponse<Response>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Kindly enter your Notification ID in the query string",
                    };

                Guid notificationIdGuid = new Guid(notificationIdString);
                var checkIfNotificationExist = await _repository.NotificationRepository.GetByNotificationId(notificationIdGuid, true);

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
                    ResponseCode = "400",
                    ResponseMessage = "Error occured while updating your notification",
                };
            }
        }
    }
}
