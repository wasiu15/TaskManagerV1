﻿using TaskManager.Domain.Models;

namespace TaskManager.Application.Repository.Interfaces
{
    public interface IProjectRepository
    {
        void CreateProject(Project project);
        void UpdateProject(Project project);
        void DeleteProject(Project project);
        Task<Project> GetProjectByProjectId(string projectId, bool trackChanges);
        Task<Project> GetProjectByNameAndDescription(string projectName, string projectDescription, bool trackChanges);
        Task<List<Project>> GetProjects();
    }
}
