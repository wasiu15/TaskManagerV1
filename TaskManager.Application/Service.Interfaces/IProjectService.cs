﻿using Domain;
using TaskManager.Domain.Dtos;

namespace TaskManager.Application.Service.Interfaces
{
    public interface IProjectService
    {
        Task<GenericResponse<IEnumerable<ProjectResponse>>> GetAllProjects();
        Task<GenericResponse<ProjectDto>> GetProjectByProjectId(string ProjectId);
        Task<GenericResponse<ProjectResponse>> CreateProject(CreateProjectRequest task);
        Task<GenericResponse<ProjectResponse>> AssignTask(string projectId, AddOrDelete operation, string taskId);
        Task<GenericResponse<ProjectResponse>> UpdateProject(string ProjectIdString, CreateProjectRequest request);
        Task<GenericResponse<ProjectResponse>> DeleteProject(string ProjectId);
    }
}
