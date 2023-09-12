using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Repository.Interfaces;
using TaskManager.Application.Service.Interfaces;

namespace TaskManager.Infrastructure.Services
{
    internal sealed class TaskService : ITaskService
    {
        private readonly IRepositoryManager _repository;

        public TaskService(IRepositoryManager repositoryManager)
        {
            _repository = repositoryManager;
        }
    }
}
