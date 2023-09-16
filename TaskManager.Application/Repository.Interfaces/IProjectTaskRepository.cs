using TaskManager.Domain.Dtos;
using TaskManager.Domain.Models;

namespace TaskManager.Application.Repository.Interfaces
{
    public interface IProjectTaskRepository
    {
        void CreateProjectTask(ProjectUserTask projectTaskDto);
        Task<IEnumerable<ProjectUserTask>> GetAll();
        Task<IEnumerable<ProjectUserTask>> GetByTaskId(string taskId, bool trackChanges);
        void UpdateProjectTask(ProjectUserTask projectTaskDto);
        void DeleteProjectTask(ProjectUserTask projectTaskDto);
        void DeleteProjectUserTasks(IEnumerable<ProjectUserTask> listOfProjectTasks);
        Task<ProjectUserTask> GetByProjectIdAndTaskId(string projectId, string taskId, bool trackChanges);
        void DeleteProjectTaskByProjectId(string projectId);
        void DeleteProjectTaskByTaskId(string taskId);
        Task<IEnumerable<string>> GetTaskIdsFromProjectId(string projectId);
    }
}
