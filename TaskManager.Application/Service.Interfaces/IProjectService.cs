using Domain;
using TaskManager.Domain.Dtos;
using TaskManager.Domain.Models;

namespace TaskManager.Application.Service.Interfaces
{
    public interface IProjectService
    {
        Task<GenericResponse<IEnumerable<ProjectResponse>>> GetAllProjects();
        Task<GenericResponse<List<TaskResponse>>> GetProjectByProjectId(string ProjectId);
        Task<GenericResponse<Project>> CreateProject(CreateProjectRequest task);
        Task<GenericResponse<ProjectResponse>> AssignTask(string projectId, AddOrDelete operation, string taskId);
        Task<GenericResponse<ProjectResponse>> UpdateProject(string ProjectIdString, CreateProjectRequest request);
        Task<GenericResponse<ProjectResponse>> DeleteProject(string ProjectId);
    }
}
