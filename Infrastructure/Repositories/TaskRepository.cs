using Domain.Models;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Repository.Interfaces;

namespace TaskManager.Infrastructure.Repositories
{
    public class TaskRepository : RepositoryBase<UserTask>, ITaskRepository
    {
        public TaskRepository(RepositoryContext context) : base(context)
        {

        }

        public void AddTask(UserTask task)
        {
            throw new NotImplementedException();
        }

        public void DeleteTask(UserTask task)
        {
            throw new NotImplementedException();
        }

        public Task<UserTask> GetTask(UserTask task)
        {
            throw new NotImplementedException();
        }

        public void UpdateTask(UserTask task)
        {
            throw new NotImplementedException();
        }
    }
}
