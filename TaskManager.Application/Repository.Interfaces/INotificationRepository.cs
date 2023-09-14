using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Application.Repository.Interfaces
{
    public interface INotificationRepository
    {
        void CreateNotification(Notification notification);
        void UpdateNotification(Notification notification);
        void DeleteNotification(Notification notification);
        Task<Notification> GetByNotificationId(Guid notificationId, bool trackChanges);
        Task<List<Notification>> GetNotifications();
    }
}
