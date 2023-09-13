using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Dtos;

namespace TaskManager.Application.Repository.Interfaces
{
    public interface IProjectRepository
    {
        void CreateProject(Project project);
        void UpdateProject(Project project);
        void DeleteProject(Project project);
        Task<Project> GetProjectByProjectId(Guid projectId, bool trackChanges);
        Task<IEnumerable<Project>> GetProjects();
    }
}
