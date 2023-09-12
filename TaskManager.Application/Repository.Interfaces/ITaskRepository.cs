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
        Task<IEnumerable<UserTask>> GetTasks();
    }
}
