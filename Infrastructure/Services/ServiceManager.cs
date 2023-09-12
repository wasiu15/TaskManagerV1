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

        public ServiceManager(IRepositoryManager repositoryManager)
        {
            _taskService = new Lazy<ITaskService>(() => new TaskService(repositoryManager));
        }

        public ITaskService TaskService => _taskService.Value;
    }
}
