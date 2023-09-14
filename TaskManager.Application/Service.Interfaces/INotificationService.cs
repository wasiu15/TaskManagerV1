using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Dtos;

namespace TaskManager.Application.Service.Interfaces
{
    public interface INotificationService
    {
        Task<GenericResponse<IEnumerable<Notification>>> GetAllNotifications();
        Task<GenericResponse<NotificationDto>> GetByNotificationId(string notificationIdString);
        Task<GenericResponse<Response>> CreateNotification(CreateNotificationRequest task);
        Task<GenericResponse<Response>> DeleteNotification(string notificationId);
        Task<GenericResponse<Response>> ReadOrUnread(string notificationIdString);
    }
}
