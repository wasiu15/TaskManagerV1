using Domain.Models;
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
