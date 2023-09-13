using Domain.Models;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Repository.Interfaces;
using TaskManager.Domain.Dtos;

namespace TaskManager.Infrastructure.Repositories
{
    public class ProjectRepository: RepositoryBase<Project>, IProjectRepository
    {
        public ProjectRepository(RepositoryContext context) : base(context)
        {

        }

        public void CreateProject(Project project) => Create(project);
        public void UpdateProject(Project project) => Update(project);
        public void DeleteProject(Project project) => Delete(project);
        public async Task<Project> GetProjectByProjectId(Guid projectId, bool trackChanges) => await FindByCondition(x => x.ProjectId.Equals(projectId), trackChanges).FirstOrDefaultAsync();
        public async Task<IEnumerable<Project>> GetProjects() => await FindAll(false).ToListAsync();
    }
}
