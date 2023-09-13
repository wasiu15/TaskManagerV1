using Domain;
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
    public class TaskRepository : RepositoryBase<UserTask>, ITaskRepository
    {
        public TaskRepository(RepositoryContext context) : base(context)
        {

        }

        public void CreateTask(UserTask task) => Create(task);
        public void UpdateTask(UserTask task) => Update(task);
        public void DeleteTask(UserTask task) => Delete(task);
        public async Task<IEnumerable<UserTask>> GetTasksDueThisWeek(DateOnly todayDate, DateOnly lastDayOfCurrentWeek, bool trackChanges) => await FindByCondition(x => x.DueDate < lastDayOfCurrentWeek && x.DueDate >= todayDate, trackChanges).ToListAsync();
        public async Task<UserTask> GetTaskByTaskId(Guid taskId, bool trackChanges) => await FindByCondition(x => x.TaskId.Equals(taskId), trackChanges).FirstOrDefaultAsync();
        public async Task<IEnumerable<UserTask>> GetTasks() => await FindAll(false).ToListAsync();
        public async Task<IEnumerable<UserTask>> GetTasksByStatusOrPriority(Status status, Priority priority, bool trackChanges) => await FindByCondition(x => x.Status.Equals(status) || x.Priority.Equals(priority), trackChanges).ToListAsync();

    }
}
