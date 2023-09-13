using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Dtos;

namespace TaskManager.Application.Service.Interfaces
{
    public interface IProjectService
    {
        Task<GenericResponse<IEnumerable<ProjectResponse>>> GetAllProjects();
        Task<GenericResponse<ProjectDto>> GetProjectByProjectId(string ProjectId);
        Task<GenericResponse<ProjectResponse>> CreateProject(CreateProjectRequest task);
        Task<GenericResponse<ProjectResponse>> AssignTask(string projectId, string taskId);
        Task<GenericResponse<ProjectResponse>> UpdateProject(string ProjectIdString, CreateProjectRequest request);
        Task<GenericResponse<ProjectResponse>> DeleteProject(string ProjectId);
    }
}
