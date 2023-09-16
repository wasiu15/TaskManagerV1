using TaskManager.Domain.Dtos;
using TaskManager.Domain.Models;

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
        Task<GenericResponse<IEnumerable<TaskResponse>>> GetAllTasksByUserId();
    }
}
