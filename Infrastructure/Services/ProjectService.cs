﻿using Domain;
using Domain.Models;
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
                    };

                //  LOOP ALL THE PROJECTS INTO THE RESPONSE FIELD
                var response = new List<ProjectResponse>();
                foreach (var project in allProjects)
                {
                    response.Add(new ProjectResponse()
                    {
                        Id = project.ProjectId.ToString(),
                        Name = project.Name,
                        Description = project.Description
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
                    ResponseCode = "500",
                    ResponseMessage = "Error occured while getting projects",
                };
            }

        }

        public async Task<GenericResponse<ProjectResponse>> AssignTask(string projectId, AddOrDelete operation, string taskId)
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
                    };

                //  CHECK IF PROJECT EXIST IN DATABASE
                var getProjectFromDb = await _repository.ProjectRepository.GetProjectByProjectId(taskId, true);
                if (getProjectFromDb == null)
                    return new GenericResponse<ProjectResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Project not exist",
                    };

                //  CHECK IF TASK EXIST IN DATABASE
                var checkIfTaskExistInUserTaskDb = await _repository.TaskRepository.GetTaskByTaskId(taskId, false);

                if (checkIfTaskExistInUserTaskDb == null)
                {
                    return new GenericResponse<ProjectResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "The task you just assigned does not exist",
                    };
                }
                else
                {
                    //  TRANSFER ALL CURRENT TASKS IN THE PROJECT INTO A NEW VARIABLE SO IT WILL BE MANIPULATED EASILY
                    List<UserTask> getProjectTasks = (List<UserTask>)await _repository.TaskRepository.GetTasksByProjectId(projectId, true);
                    
                    //  THIS CONDITION WILL CHECK IF WE NEED TO ADD OR DELETE THE TASK
                    if (operation == AddOrDelete.Add)
                    {
                        //  CHECK IF THE TASK TO ADD ALREADY EXIST
                        if (Util.IsListContainTask(getProjectTasks, checkIfTaskExistInUserTaskDb))
                        {
                            return new GenericResponse<ProjectResponse>
                            {
                                IsSuccessful = false,
                                ResponseCode = "400",
                                ResponseMessage = "This task exist in your project already",
                            };
                        }
                        else
                        {
                            //  ADD THE NEW TASK TO THE ARRAY AND SAVE TO THE DB
                            getProjectTasks.Add(checkIfTaskExistInUserTaskDb);
                        }
                    }
                    else if (operation == AddOrDelete.Delete)
                    {
                        //  CHECK IF THE TASK TO ADD ALREADY EXIST
                        if (Util.IsListContainTask(getProjectTasks, checkIfTaskExistInUserTaskDb))
                        {
                            //  ADD THE NEW TASK TO THE ARRAY AND SAVE TO THE DB
                            getProjectTasks.RemoveAll(x => x.TaskId == checkIfTaskExistInUserTaskDb.TaskId);
                        }
                        else
                        {
                            return new GenericResponse<ProjectResponse>
                            {
                                IsSuccessful = false,
                                ResponseCode = "400",
                                ResponseMessage = "This task does not exist in your project",
                            };
                        }
                    }

                    getProjectFromDb.UserTasks = getProjectTasks;
                    _repository.ProjectRepository.UpdateProject(getProjectFromDb);
                    await _repository.SaveAsync();

                }
                return new GenericResponse<ProjectResponse>
                {
                    IsSuccessful = true,
                    ResponseCode = "200",
                    ResponseMessage = "Your project tasks have been successfully updated",
                };
            }
            catch(Exception ex)
            {
                return new GenericResponse<ProjectResponse>
                {
                    IsSuccessful = false,
                    ResponseCode = "500",
                    ResponseMessage = "Error occured while adding new task to this project",
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
                    };

                //  CHECK IF PROJECT ALREADY EXIST
                var isProjectExist = await _repository.ProjectRepository.GetProjectByNameAndDescription(project.Name, project.Description, false);
                if (isProjectExist != null)
                    return new GenericResponse<ProjectResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Sorry, project already exist",
                    };

                //  CHECK PROJECT OBJECT WHICH WE WILL SEND TO THE DATABASE BEFORE THE PROJECT MODEL IS OUR ENTITY
                Project projectToSave = new Project
                {
                    ProjectId = Guid.NewGuid().ToString(),
                    Name = project.Name,
                    Description = project.Description,
                };
                _repository.ProjectRepository.CreateProject(projectToSave);
                await _repository.SaveAsync();

                return new GenericResponse<ProjectResponse>
                {
                    IsSuccessful = true,
                    ResponseCode = "201",
                    ResponseMessage = "You just successfully created a new project",
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<ProjectResponse>
                {
                    IsSuccessful = false,
                    ResponseCode = "500",
                    ResponseMessage = "Error occured while creating your new project",
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
                    };

                var checkIfProjectExist = await _repository.ProjectRepository.GetProjectByProjectId(projectId, true);

                //  CHECK IF THE PROJECT EXIST
                if (checkIfProjectExist == null)
                    return new GenericResponse<ProjectResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Project not found",
                    };

                _repository.ProjectRepository.DeleteProject(checkIfProjectExist);
                await _repository.SaveAsync();

                return new GenericResponse<ProjectResponse>
                {
                    IsSuccessful = true,
                    ResponseCode = "200",
                    ResponseMessage = "Successfully deleted your project from the database",
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<ProjectResponse>
                {
                    IsSuccessful = false,
                    ResponseCode = "500",
                    ResponseMessage = "Error occured while deleting your project",
                };
            }
        }

        public async Task<GenericResponse<ProjectDto>> GetProjectByProjectId(string projectId)
        {
            try
            {
                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if (string.IsNullOrEmpty(projectId))
                    return new GenericResponse<ProjectDto>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Kindly enter your project Id in the query string",
                    };

                // THIS WILL GET ALL TASKS FROM THE REPOSITORY
                var responseFromDb = await _repository.ProjectRepository.GetProjectByProjectId(projectId, false);

                //  CHECK IF PROJECT EXIST
                if (responseFromDb == null)
                    return new GenericResponse<ProjectDto>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Project not found",
                    };

                //  FETCH ALL ASSIGNED TASKED BASED OF THE PRODUCT ID WHICH IS OUR FOREIGN KEY
                var getAssignedTasks = await _repository.TaskRepository.GetTasksByProjectId(projectId, false);
                
                //  MAP THE REQUIRED DATA WE NEED THE USERS TO SEE INTO A NEW DTO WHICH IS THE TASKDTO FOR THE RESPONSE
                List<TaskDto> assignedTasksDto = new List<TaskDto>();
                foreach (var task in getAssignedTasks)
                {
                    assignedTasksDto.Add(new TaskDto
                    {
                        Title = task.Title,
                        Description = task.Description,
                        DueDate = task.DueDate,
                        Priority = task.Priority,
                        Status = task.Status
                    });
                }

                //  THIS IS THE R   ESPONSE DATA TO SEND BACK TO OUR CONSUMER
                var response = new ProjectDto()
                {
                    Name = responseFromDb.Name, 
                    Description = responseFromDb.Description,
                    AssociatedTasks = assignedTasksDto.ToArray()
                };

                return new GenericResponse<ProjectDto>
                {
                    IsSuccessful = true,
                    ResponseCode = "200",
                    ResponseMessage = "Successfully fetched project and the total tasks attached is: "+ getAssignedTasks.Count(),
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<ProjectDto>
                {
                    IsSuccessful = false,
                    ResponseCode = "500",
                    ResponseMessage = "Error occured while getting your project",
                };
            }
        }

        public async Task<GenericResponse<ProjectResponse>> UpdateProject(string projectId, CreateProjectRequest request)
        {
            try
            {
                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if (string.IsNullOrEmpty(projectId))
                    return new GenericResponse<ProjectResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Kindly enter your project Id in the query string",
                    };

                var checkIfProjectExist = await _repository.ProjectRepository.GetProjectByProjectId(projectId, true);

                //  CHECK IF THE TASK EXIST
                if (checkIfProjectExist == null)
                    return new GenericResponse<ProjectResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Project not found",
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
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<ProjectResponse>
                {
                    IsSuccessful = false,
                    ResponseCode = "500",
                    ResponseMessage = "Error occured while updating your project",
                };
            }
        }
    }
}