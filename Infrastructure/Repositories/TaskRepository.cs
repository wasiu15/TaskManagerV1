using Domain;
using TaskManager.Domain.Models;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IEnumerable<UserTask>> GetTasksDueThisWeek(DateTime todayDate, DateTime lastDayOfCurrentWeek, bool trackChanges) => await FindByCondition(x => x.DueDate.Date <= lastDayOfCurrentWeek.Date && x.DueDate.Date >= todayDate.Date, trackChanges).ToListAsync();
        public async Task<UserTask> GetTaskByTaskId(string taskId, bool trackChanges) => await FindByCondition(x => x.Id.Equals(taskId), trackChanges).FirstOrDefaultAsync();
        public async Task<IEnumerable<UserTask>> GetTasks() => await FindAll(false).ToListAsync();
        public async Task<IEnumerable<UserTask>> GetTasksByStatusOrPriority(Status status, Priority priority, bool trackChanges) => await FindByCondition(x => x.Status.Equals(status) || x.Priority.Equals(priority), trackChanges).ToListAsync();
        //public async Task<IEnumerable<UserTask>> GetTasksByProjectId(string projectId, bool trackChanges) => await FindByCondition(x => x.Equals(projectId), trackChanges).ToListAsync();
        public async Task<IEnumerable<UserTask>> GetTasksByUserId(string userId, bool trackChanges) => await FindByCondition(x => x.UserId.Equals(userId), trackChanges).ToListAsync();
        public async Task<IEnumerable<UserTask>> GetAnyUnCompletedTaskToDueInTwoDays(bool trackChanges) => await FindByCondition(x => x.Status != Status.completed && Util.IsDateDue(x.DueDate), trackChanges).ToListAsync();

        public async Task<IEnumerable<UserTask>> GetTasksByArrayOfTaskIds(List<string> taskIds, bool trackChanges)
        {
            List<UserTask> data = new List<UserTask>();
            foreach (var taskId in taskIds)
            {
                var taskObj = await FindByCondition(x => x.Id.Equals(taskId), false).FirstOrDefaultAsync();
                data.Add(taskObj);
            }
            return data;
        }
    }
}