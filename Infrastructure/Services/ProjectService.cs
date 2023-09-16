using Domain;
using TaskManager.Domain.Models;
using TaskManager.Application.Repository.Interfaces;
using TaskManager.Application.Service.Interfaces;
using TaskManager.Domain.Dtos;
using System.Threading.Tasks;

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
                        Id = project.Id.ToString(),
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
                var getProjectFromDb = await _repository.ProjectRepository.GetProjectByProjectId(projectId, true);
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
                    //  GET FOREIGN KEY TO CHECKING FOR CONFIRMATION BEFORE ADDING OR DELETING IT
                    var checkIfForeignKeyExist = await _repository.ProjectTaskRepository.GetByProjectIdAndTaskId(projectId, taskId, true);

                    //  CREATE THE FOREIGN KEY OBJECT
                    var projectUserTask = new ProjectUserTask
                    {
                        ProjectId = projectId,
                        UserTaskId = taskId
                    };

                    //  THIS CONDITION WILL CHECK IF WE NEED TO ADD OR DELETE THE TASK
                    if (operation == AddOrDelete.Add)
                    {
                        if (checkIfForeignKeyExist == null)
                        {
                            //  THIS WILL CREATE A FOREIGN KEY TO LINK THE TASK TO THE PROJECT
                            _repository.ProjectTaskRepository.CreateProjectTask(projectUserTask);
                        }
                        else
                        {
                            return new GenericResponse<ProjectResponse>
                            {
                                IsSuccessful = false,
                                ResponseCode = "400",
                                ResponseMessage = "This task was assigned to this project already",
                            };
                        }
                    }
                    else if (operation == AddOrDelete.Delete)
                    {
                        if (checkIfForeignKeyExist != null)
                        {
                            //  ADD THE NEW TASK TO THE ARRAY AND SAVE TO THE DB
                            _repository.ProjectTaskRepository.DeleteProjectTask(checkIfForeignKeyExist);
                        }
                        else
                        {
                            return new GenericResponse<ProjectResponse>
                            {
                                IsSuccessful = false,
                                ResponseCode = "400",
                                ResponseMessage = "This task does not belong to this project",
                            };
                        }
                    }

                    //  SAVE INTO THE DATABASE
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

        public async Task<GenericResponse<Project>> CreateProject(CreateProjectRequest project)
        {
            try
            {
                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if (string.IsNullOrEmpty(project.Name) || string.IsNullOrEmpty(project.Description))
                    return new GenericResponse<Project>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Please, enter all fields",
                    };

                //  CHECK IF PROJECT ALREADY EXIST
                var isProjectExist = await _repository.ProjectRepository.GetProjectByNameAndDescription(project.Name, project.Description, false);
                if (isProjectExist != null)
                    return new GenericResponse<Project>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Sorry, project already exist",
                    };

                //  CHECK PROJECT OBJECT WHICH WE WILL SEND TO THE DATABASE BEFORE THE PROJECT MODEL IS OUR ENTITY
                Project projectToSave = new Project
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = project.Name,
                    Description = project.Description,
                };
                _repository.ProjectRepository.CreateProject(projectToSave);
                await _repository.SaveAsync();

                return new GenericResponse<Project>
                {
                    IsSuccessful = true,
                    ResponseCode = "201",
                    ResponseMessage = "You just successfully created a new project",
                    Data = projectToSave
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Project>
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

                //  CHECK IF THIS PROJECT IS RELATED TO ANY TASK
                var listOfRelatedTasksByProjectId = await _repository.ProjectTaskRepository.GetByProjectId(projectId, true);
                //  DELETE FOREIGN KEYS IF FOUND
                if (listOfRelatedTasksByProjectId != null)
                {
                    //  DELETE FOREIGN KEY(S) FROM DATABASE
                    _repository.ProjectTaskRepository.DeleteProjectUserTasks(listOfRelatedTasksByProjectId);
                }
                
                //  DELETE FROM PROJECT DATABASE
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

        public async Task<GenericResponse<List<TaskResponse>>> GetProjectByProjectId(string projectId)
        {
            try
            {
                //  CHECK IF REQUIRED INPUTS ARE ENTERED
                if (string.IsNullOrEmpty(projectId))
                    return new GenericResponse<List<TaskResponse>>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Kindly enter your project Id in the query string",
                    };

                // THIS WILL GET ALL TASKS FROM THE REPOSITORY
                var responseFromDb = await _repository.ProjectRepository.GetProjectByProjectId(projectId, false);
                //  CHECK IF PROJECT EXIST
                if (responseFromDb == null)
                    return new GenericResponse<List<TaskResponse>>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "Project not found",
                    };

                //  FETCH ALL ASSIGNED TASKS ID BASED ON THE PRODUCT ID WHICH IS OUR FOREIGN KEY
                var getAssignedTasksIds = await _repository.ProjectTaskRepository.GetTaskIdsFromProjectId(projectId);
                if (getAssignedTasksIds == null)
                {
                    return new GenericResponse<List<TaskResponse>>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "No tasks are assigned to your project yet",
                    };
                }

                //  IF IT PREVIOUS CONDITION DOES NOT RETURN NULL THEN FETCH ALL ASSIGNED TASKS ID BASED ON THE TASK IDS WE RECIEVED FROM THE ABOVE CODE (getAssignedTasksIds variable)
                var assignedTasks = await _repository.TaskRepository.GetTasksByArrayOfTaskIds(getAssignedTasksIds.ToList(), false);
                if(assignedTasks == null)
                {
                    return new GenericResponse<List<TaskResponse>>
                    {
                        IsSuccessful = false,
                        ResponseCode = "400",
                        ResponseMessage = "No tasks are assigned to your project yet",
                    };
                }

                //  MAP THE REQUIRED DATA WE NEED THE USERS TO SEE INTO A NEW DTO WHICH IS THE TASKDTO FOR THE RESPONSE
                List<TaskResponse> assignedTasksResponse = new List<TaskResponse>();
                foreach (var task in assignedTasks)
                {
                    assignedTasksResponse.Add(new TaskResponse
                    {
                        Id = task.Id,
                        Title = task.Title,
                        Description = task.Description,
                        DueDate = task.DueDate.ToShortDateString(),
                        Priority = task.Priority.ToString(),
                        Status = task.Status.ToString()
                    });
                }


                return new GenericResponse<List<TaskResponse>>
                {
                    IsSuccessful = true,
                    ResponseCode = "200",
                    ResponseMessage = "Successfully fetched project and the total tasks attached is: "+ assignedTasksResponse.Count(),
                    Data = assignedTasksResponse
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<List<TaskResponse>>
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