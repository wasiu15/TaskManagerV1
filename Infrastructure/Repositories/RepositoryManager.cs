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
        private readonly Lazy<IProjectRepository> _projectRepository;

        public RepositoryManager(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
            _taskRepository = new Lazy<ITaskRepository>(() => new TaskRepository(repositoryContext));
            _projectRepository = new Lazy<IProjectRepository>(() => new ProjectRepository(repositoryContext));
        }

        public ITaskRepository TaskRepository => _taskRepository.Value;
        public IProjectRepository ProjectRepository => _projectRepository.Value;

        public async Task SaveAsync() => _repositoryContext.SaveChanges();
    }
}
