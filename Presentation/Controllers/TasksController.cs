using Domain;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Service.Interfaces;
using TaskManager.Domain.Dtos;

namespace TaskManager.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public TasksController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet("getall")]
        public async Task<ActionResult> GetAll()
        {
            var response = await _serviceManager.TaskService.GetAllTasks();
            return Ok(response);
        }

        [HttpGet("getTaskById")]
        public async Task<ActionResult> GetTaskById([FromQuery] string taskId)
        {
            var response = await _serviceManager.TaskService.GetTaskByTaskId(taskId);
            return Ok(response);
        }

        [HttpGet("getByStatusOrPriority")]
        public async Task<ActionResult> GetByStatusOrPriority([FromQuery] Status status, [FromQuery] Priority priority)
        {
            StatusAndPriorityRequest request = new StatusAndPriorityRequest()
            {
                TaskStatus = status,
                TaskPriority = priority
            };
            var response = await _serviceManager.TaskService.GetTaskByPriorityOrStatus(request);
            return Ok(response);
        }

        [HttpGet("getTasksDueThisWeek")]
        public async Task<ActionResult> GetTasksDueThisWeek()
        {
            var response = await _serviceManager.TaskService.GetTasksDueThisWeek();
            return Ok(response);
        }

        [HttpPost("addTask")]
        public async Task<ActionResult> AddTask(CreateTaskRequest task)
        {
            var response = await _serviceManager.TaskService.CreateTask(task);
            return Ok(response);
        }

        [HttpPatch("UpdateTask")]
        public async Task<ActionResult> UpdateTask([FromQuery] string taskId, StatusAndPriorityRequest request)
        {
            var response = await _serviceManager.TaskService.UpdateTask(taskId, request);
            return Ok(response);
        }

        [HttpDelete("DeleteTask")]
        public async Task<ActionResult> DeleteTask([FromQuery] string taskId)
        {
            var response = await _serviceManager.TaskService.DeleteTask(taskId);
            return Ok(response);
        }
    }
}