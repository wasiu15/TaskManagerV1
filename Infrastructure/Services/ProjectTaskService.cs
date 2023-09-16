using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Repository.Interfaces;
using TaskManager.Application.Service.Interfaces;
using TaskManager.Domain.Dtos;
using TaskManager.Domain.Models;

namespace TaskManager.Infrastructure.Services
{
    public class ProjectTaskService : IProjectTaskService
    {
        private readonly IRepositoryManager _repository;

        public ProjectTaskService(IRepositoryManager repositoryManager)
        {
            _repository = repositoryManager;
        }

        public async Task<GenericResponse<ProjectTaskDto>> CreateProjectTask(ProjectUserTask projectTask)
        {
            try
            {
                //  CHECK IF PROJECT ALREADY EXIST
                var isProjectTaskExist = await _repository.ProjectTaskRepository.GetByProjectIdAndTaskId(projectTask.ProjectId, projectTask.UserTaskId, false);
                if (isProjectTaskExist != null)
                    return new GenericResponse<ProjectTaskDto>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Foreign key alreay exist",
                    };

                _repository.ProjectTaskRepository.CreateProjectTask(projectTask);
                await _repository.SaveAsync();

                return new GenericResponse<ProjectTaskDto>
                {
                    IsSuccessful = true,
                    ResponseCode = "201",
                    ResponseMessage = "Foreign key added successfully",
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<ProjectTaskDto>
                {
                    IsSuccessful = false,
                    ResponseCode = "500",
                    ResponseMessage = "Something went wrong while adding foreign key",
                };
            }
        }

        public async Task<GenericResponse<ProjectTaskDto>> DeleteProjectTask(ProjectTaskDto projectTask)
        {
            try
            {
                //  CHECK IF PROJECT ALREADY EXIST
                var isProjectTaskExist = await _repository.ProjectTaskRepository.GetByProjectIdAndTaskId(projectTask.ProjectId, projectTask.UserTaskId, true);
                if (isProjectTaskExist != null)
                    return new GenericResponse<ProjectTaskDto>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "foreign key not found",
                    };

                _repository.ProjectTaskRepository.DeleteProjectTask(isProjectTaskExist);
                await _repository.SaveAsync();

                return new GenericResponse<ProjectTaskDto>
                {
                    IsSuccessful = true,
                    ResponseCode = "200",
                    ResponseMessage = "Foreign key deleted successfully",
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<ProjectTaskDto>
                {
                    IsSuccessful = false,
                    ResponseCode = "500",
                    ResponseMessage = "Something went wrong while deleting foreign key from the database",
                };
            }
        }

        public async Task<GenericResponse<ProjectUserTask>> GetByProjectIdAndTaskId(ProjectTaskDto projectTask)
        {
            try
            {
                var responseFromDb = await _repository.ProjectTaskRepository.GetByProjectIdAndTaskId(projectTask.ProjectId, projectTask.UserTaskId, false);

                if (responseFromDb == null)
                    return new GenericResponse<ProjectUserTask>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Foreign key not found",
                    };

                return new GenericResponse<ProjectUserTask>
                {
                    IsSuccessful = true,
                    ResponseCode = "200",
                    ResponseMessage = "Successfully fetched foreign key from the database",
                    Data = responseFromDb
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<ProjectUserTask>
                {
                    IsSuccessful = false,
                    ResponseCode = "500",
                    ResponseMessage = "Error occured while getting foreign key",
                };
            }
        }

        public async Task<GenericResponse<ProjectTaskDto>> UpdateProjectTask(ProjectTaskDto request)
        {
            try
            {
                var responseFromDb = await _repository.ProjectTaskRepository.GetByProjectIdAndTaskId(request.ProjectId, request.UserTaskId, false);
                if (responseFromDb == null)
                    return new GenericResponse<ProjectTaskDto>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Foreign key not found",
                    };

                _repository.ProjectTaskRepository.UpdateProjectTask(responseFromDb);
                await _repository.SaveAsync();

                return new GenericResponse<ProjectTaskDto>
                {
                    IsSuccessful = true,
                    ResponseCode = "200",
                    ResponseMessage = "Successfully updated foreign key in the database",
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<ProjectTaskDto>
                {
                    IsSuccessful = false,
                    ResponseCode = "500",
                    ResponseMessage = "Error occured while updating foreign key",
                };
            }
        }
    }
}