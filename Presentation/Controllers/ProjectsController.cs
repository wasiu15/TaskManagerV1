﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Service.Interfaces;
using TaskManager.Domain.Dtos;

namespace TaskManager.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public ProjectsController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet("getall")]
        public async Task<ActionResult> GetAll()
        {
            var response = await _serviceManager.ProjectService.GetAllProjects();
            return Ok(response);
        }

        [HttpGet("getProjectById")]
        public async Task<ActionResult> GetProjectById([FromQuery] string projectId)
        {
            var response = await _serviceManager.TaskService.GetTaskByTaskId(projectId);
            return Ok(response);
        }

        [HttpPost("addProject")]
        public async Task<ActionResult> AddProject(CreateProjectRequest project)
        {
            var response = await _serviceManager.ProjectService.CreateProject(project);
            return Ok(response);
        }

        [HttpPatch("updateProject")]
        public async Task<ActionResult> UpdateProject(CreateProjectRequest project)
        {
            var response = await _serviceManager.ProjectService.CreateProject(project);
            return Ok(response);
        }

        [HttpPatch("assignProject")]
        public async Task<ActionResult> AssignProject([FromQuery] string projectId,[FromBody] AssignTaskRequest request)
        {
            var response = await _serviceManager.ProjectService.AssignTask(projectId, request.TaskId);
            return Ok(response);
        }

        [HttpDelete("DeleteProject")]
        public async Task<ActionResult> DeleteProject([FromQuery] string projectId)
        {
            var response = await _serviceManager.ProjectService.DeleteProject(projectId);
            return Ok(response);
        }
    }
}