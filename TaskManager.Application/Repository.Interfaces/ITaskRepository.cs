using Domain;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Application.Repository.Interfaces
{
    public interface ITaskRepository
    {
        void CreateTask(UserTask task);
        void UpdateTask(UserTask task);
        void DeleteTask(UserTask task);
        Task<UserTask> GetTaskByTaskId(Guid taskId, bool trackChanges);
        Task<IEnumerable<UserTask>> GetTasksDueThisWeek(DateOnly todayDate, DateOnly lastDayOfCurrentWeek, bool trackChanges);
        Task<IEnumerable<UserTask>> GetTasks();
        Task<IEnumerable<UserTask>> GetTasksByStatusOrPriority(Status status, Priority priority, bool trackChanges);
        Task<IEnumerable<UserTask>> GetTasksByProjectId(Guid projectId, bool trackChanges);
    }
}
