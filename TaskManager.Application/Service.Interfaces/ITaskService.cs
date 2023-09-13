using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Dtos;

namespace TaskManager.Application.Service.Interfaces
{
    public interface ITaskService
    {
        Task<GenericResponse<IEnumerable<TaskResponse>>> GetAllTasks();
        Task<GenericResponse<TaskResponse>> GetTaskByTaskId(string taskId);
        Task<GenericResponse<IEnumerable<TaskResponse>>> GetTasksDueThisWeek();
        Task<GenericResponse<IEnumerable<TaskResponse>>> GetTaskByPriorityOrStatus(StatusAndPriorityRequest request);
        Task<GenericResponse<TaskResponse>> CreateTask(CreateTaskRequest task);
        Task<GenericResponse<TaskResponse>> UpdateTask(string taskIdString, StatusAndPriorityRequest request);
        Task<GenericResponse<TaskResponse>> DeleteTask(string taskId);
    }
}
