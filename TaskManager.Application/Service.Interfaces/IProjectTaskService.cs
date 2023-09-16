using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Dtos;
using TaskManager.Domain.Models;

namespace TaskManager.Application.Service.Interfaces
{
    public interface IProjectTaskService
    {
        Task<GenericResponse<ProjectTaskDto>> CreateProjectTask(ProjectUserTask projectTask);
        Task<GenericResponse<ProjectTaskDto>> DeleteProjectTask(ProjectTaskDto projectTask);
        Task<GenericResponse<ProjectTaskDto>> UpdateProjectTask(ProjectTaskDto projectTask);
        Task<GenericResponse<ProjectUserTask>> GetByProjectIdAndTaskId(ProjectTaskDto projectTask);
    }
}
