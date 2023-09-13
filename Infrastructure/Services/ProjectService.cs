using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Repository.Interfaces;
using TaskManager.Application.Service.Interfaces;
using TaskManager.Domain.Dtos;

namespace TaskManager.Infrastructure.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IRepositoryManager _repository;

        public ProjectService(IRepositoryManager repositoryManager)
        {
            _repository = repositoryManager;
        }

        public async Task<GenericResponse<IEnumerable<ProjectResponse>>> GetAllProjects()
        {
            try
            {                
                // THIS WILL GET ALL TASKS FROM THE REPOSITORY
                var allProjects = await _repository.ProjectRepository.GetProjects();
                
                //  CHECK IF THE LIST IS EMPTY
                if (allProjects.Count() == 0)
                    return new GenericResponse<IEnumerable<ProjectResponse>>
                    {
                        IsSuccessful = true,
                        ResponseCode = "200",
                        ResponseMessage = "No projects found",
                        Data = null
                    };

                var response = new List<ProjectResponse>();
                foreach (var project in allProjects)
                {
                    response.Add(new ProjectResponse()
                    {
                        Id = project.ProjectId.ToString(),
                        Name = project.Name,
                        Description = project.Description,
                        AssociatedTasks = project.Tasks
                    });
                }
                
                return new GenericResponse<IEnumerable<ProjectResponse>>
                {
                    IsSuccessful = true,
                    ResponseCode = "200",
                    ResponseMessage = "Successfully fetched all projects. Total number: "+allProjects.Count(),
                    Data = response
                };
            } 
            catch (Exception ex)
            {
                return new GenericResponse<IEnumerable<ProjectResponse>>
                {
                    IsSuccessful = false,
                    ResponseCode = "400",
                    ResponseMessage = "Error occured while getting projects",
                    Data = null
                };
            }

        }

        public async Task<GenericResponse<ProjectResponse>> AssignTask(string projectId, string taskId)
        {
            try
            {
                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if (string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(taskId))
                    return new GenericResponse<ProjectResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Kindly enter all field",
                        Data = null
                    };

                //  CHECK IF TASK EXIST IN DATABASE
                Guid taskIdGuid = new Guid(taskId);
                var getTaskFromDb = await _repository.TaskRepository.GetTaskByTaskId(taskIdGuid, false);
                if (getTaskFromDb == null)
                    return new GenericResponse<ProjectResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "The task you just assigned does not exist",
                        Data = null
                    };

                //  CHECK IF TASK EXIST IN DATABASE
                Guid projectIdGuid = new Guid(projectId);
                var getProjectFromDb = await _repository.ProjectRepository.GetProjectByProjectId(projectIdGuid, false);
                if (getProjectFromDb == null)
                    return new GenericResponse<ProjectResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Project not exist",
                        Data = null
                    };

                //  TRANSFER ALL CURRENT TASKS IN THE PROJECT INTO A NEW VARIABLE SO IT WILL BE MANIPULATED EASILY
                List<UserTask> getProjectTasks = new List<UserTask>();
                if (getProjectFromDb.Tasks != null)
                    foreach (var task in getProjectFromDb.Tasks)
                    {
                        getProjectTasks.Add(task);
                    }

                //  CHECK IF THE TASK TO ADD ALREADY EXIST
                if (getProjectTasks.Contains(getTaskFromDb))
                    return new GenericResponse<ProjectResponse>
                    {
                        IsSuccessful = true,
                        ResponseCode = "200",
                        ResponseMessage = "This task exist in your project already",
                        Data = null
                    };

                //  ADD THE NEW TASK TO THE ARRAY AND SAVE TO THE DB
                getProjectTasks.Add(getTaskFromDb);
                getProjectFromDb.Tasks = getProjectTasks;
                _repository.ProjectRepository.UpdateProject(getProjectFromDb);
                await _repository.SaveAsync();

                return new GenericResponse<ProjectResponse>
                {
                    IsSuccessful = true,
                    ResponseCode = "200",
                    ResponseMessage = "Your new task have been added to your project",
                    Data = null
                };
            }
            catch(Exception ex)
            {
                return new GenericResponse<ProjectResponse>
                {
                    IsSuccessful = false,
                    ResponseCode = "400",
                    ResponseMessage = "Error occured while adding new task to this project",
                    Data = null
                };
            }
        }

        public async Task<GenericResponse<ProjectResponse>> CreateProject(CreateProjectRequest project)
        {
            try
            {
                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if (string.IsNullOrEmpty(project.Name) || string.IsNullOrEmpty(project.Description))
                    return new GenericResponse<ProjectResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Please, enter all fields",
                        Data = null
                    };

                List<UserTask> emptyArray = new List<UserTask>();
                Project projectToSave = new Project
                {
                    ProjectId = Guid.NewGuid(),
                    Name = project.Name,
                    Description = project.Description,
                    Tasks = emptyArray
                };
                _repository.ProjectRepository.CreateProject(projectToSave);
                await _repository.SaveAsync();

                //  CHECK IF THE LIST IS EMPTY
                return new GenericResponse<ProjectResponse>
                {
                    IsSuccessful = true,
                    ResponseCode = "201",
                    ResponseMessage = "You just successfully created a new project",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<ProjectResponse>
                {
                    IsSuccessful = false,
                    ResponseCode = "400",
                    ResponseMessage = "Error occured while creating your new project",
                    Data = null
                };
            }
        }

        public async Task<GenericResponse<ProjectResponse>> DeleteProject(string projectId)
        {
            try
            {
                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if (string.IsNullOrEmpty(projectId))
                    return new GenericResponse<ProjectResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Please, enter the project Id",
                        Data = null
                    };

                Guid projectIdGuid = new Guid(projectId);
                var checkIfProjectExist = await _repository.ProjectRepository.GetProjectByProjectId(projectIdGuid, true);

                //  CHECK IF THE PROJECT EXIST
                if (checkIfProjectExist == null)
                    return new GenericResponse<ProjectResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Project not found",
                        Data = null
                    };

                _repository.ProjectRepository.DeleteProject(checkIfProjectExist);
                await _repository.SaveAsync();

                return new GenericResponse<ProjectResponse>
                {
                    IsSuccessful = true,
                    ResponseCode = "200",
                    ResponseMessage = "Successfully deleted your project from the database",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<ProjectResponse>
                {
                    IsSuccessful = false,
                    ResponseCode = "400",
                    ResponseMessage = "Error occured while deleting your project",
                    Data = null
                };
            }
        }

        public async Task<GenericResponse<ProjectResponse>> GetProjectByProjectId(string projectIdString)
        {
            try
            {
                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if (string.IsNullOrEmpty(projectIdString))
                    return new GenericResponse<ProjectResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Kindly enter your project Id in the query string",
                        Data = null
                    };

                // THIS WILL GET ALL TASKS FROM THE REPOSITORY
                Guid projectIdGuid = new Guid(projectIdString);
                var responseFromDb = await _repository.ProjectRepository.GetProjectByProjectId(projectIdGuid, false);

                //  CHECK IF TASK EXIST
                if (responseFromDb == null)
                    return new GenericResponse<ProjectResponse>
                    {
                        IsSuccessful = true,
                        ResponseCode = "200",
                        ResponseMessage = "Project not found",
                        Data = null
                    };

                var response = new ProjectResponse()
                {
                    Id = responseFromDb.ProjectId.ToString(),
                    Name = responseFromDb.Name,
                    Description = responseFromDb.Description,
                    AssociatedTasks = responseFromDb.Tasks
                };

                return new GenericResponse<ProjectResponse>
                {
                    IsSuccessful = true,
                    ResponseCode = "200",
                    ResponseMessage = "Successfully fetched project",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<ProjectResponse>
                {
                    IsSuccessful = false,
                    ResponseCode = "400",
                    ResponseMessage = "Error occured while getting your project",
                    Data = null
                };
            }
        }

        public async Task<GenericResponse<ProjectResponse>> UpdateProject(string projectIdString, CreateProjectRequest request)
        {
            try
            {
                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if (string.IsNullOrEmpty(projectIdString))
                    return new GenericResponse<ProjectResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Kindly enter your project Id in the query string",
                        Data = null
                    };

                
                Guid projectIdGuid = new Guid(projectIdString);
                var checkIfProjectExist = await _repository.ProjectRepository.GetProjectByProjectId(projectIdGuid, true);

                //  CHECK IF THE TASK EXIST
                if (checkIfProjectExist == null)
                    return new GenericResponse<ProjectResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Project not found",
                        Data = null
                    };

                checkIfProjectExist.Name = request.Name;
                checkIfProjectExist.Description = request.Description;
                _repository.ProjectRepository.UpdateProject(checkIfProjectExist);
                await _repository.SaveAsync();

                return new GenericResponse<ProjectResponse>
                {
                    IsSuccessful = true,
                    ResponseCode = "200",
                    ResponseMessage = "Successfully updated your project in the database",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<ProjectResponse>
                {
                    IsSuccessful = false,
                    ResponseCode = "400",
                    ResponseMessage = "Error occured while updating your project",
                    Data = null
                };
            }
        }
    }
}
