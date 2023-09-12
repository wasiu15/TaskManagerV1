using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Repository.Interfaces;

namespace TaskManager.Infrastructure.Repositories
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _repositoryContext;
        private readonly Lazy<ITaskRepository> _taskRepository;

        public RepositoryManager(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
            _taskRepository = new Lazy<ITaskRepository>(() => new TaskRepository(repositoryContext));
        }

        public ITaskRepository TaskRepository => _taskRepository.Value;

        public void Save() => _repositoryContext.SaveChanges();
    }
}
