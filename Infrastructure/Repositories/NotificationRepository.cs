using Domain.Models;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Repository.Interfaces;

namespace TaskManager.Infrastructure.Repositories
{
    public class NotificationRepository : RepositoryBase<Notification>, INotificationRepository
    {
        public NotificationRepository(RepositoryContext context) : base(context)
        {

        }

        public void CreateNotification(Notification notification) => Create(notification);
        public void UpdateNotification(Notification notification) => Update(notification);
        public void DeleteNotification(Notification notification) => Delete(notification);
        public async Task<List<Notification>> GetNotifications() => await FindAll(false).ToListAsync();
        public async Task<Notification> GetByNotificationId(Guid notificationId, bool trackChanges) => await FindByCondition(x => x.NotificationId.Equals(notificationId), trackChanges).FirstOrDefaultAsync();
    }
}
