using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Repository.Interfaces;
using TaskManager.Application.Service.Interfaces;

namespace TaskManager.Infrastructure.Services
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly Lazy<ITaskService> _taskService;
        private readonly Lazy<IProjectService> _projectService;
        private readonly Lazy<IUserService> _userService;
        private readonly Lazy<INotificationService> _notificationService;


        public ServiceManager(IRepositoryManager repositoryManager, ITokenManager tokenManager)
        {
            _taskService = new Lazy<ITaskService>(() => new TaskService(repositoryManager));
            _projectService = new Lazy<IProjectService>(() => new ProjectService(repositoryManager));
            _userService = new Lazy<IUserService>(() => new UserService(repositoryManager, tokenManager));
            _notificationService = new Lazy<INotificationService>(() => new NotificationService(repositoryManager));
        }

        public ITaskService TaskService => _taskService.Value;
        public IProjectService ProjectService => _projectService.Value;
        public IUserService UserService => _userService.Value;
        public INotificationService NotificationService => _notificationService.Value;
    }
}
