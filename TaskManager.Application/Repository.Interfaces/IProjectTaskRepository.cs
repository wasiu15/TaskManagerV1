using TaskManager.Domain.Dtos;
using TaskManager.Domain.Models;

namespace TaskManager.Application.Repository.Interfaces
{
    public interface IProjectTaskRepository
    {
        void CreateProjectTask(ProjectUserTask projectTaskDto);
        void UpdateProjectTask(ProjectUserTask projectTaskDto);
        void DeleteProjectTask(ProjectUserTask projectTaskDto);
        Task<ProjectUserTask> GetByProjectIdAndTaskId(string projectId, string taskId, bool trackChanges);
        void DeleteProjectTaskByProjectId(string projectId);
        void DeleteProjectTaskByTaskId(string taskId);
        Task<IEnumerable<string>> GetTaskIdsFromProjectId(string projectId);
    }
}
