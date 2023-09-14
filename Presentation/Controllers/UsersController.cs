using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Service.Interfaces;
using TaskManager.Domain.Dtos;

namespace TaskManager.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public UsersController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet("getAllUser")]
        public async Task<ActionResult> GetAll()
        {
            var response = await _serviceManager.UserService.GetAllUsers();
            return Ok(response);
        }

        [HttpGet("getUserById")]
        public async Task<ActionResult> GetUserById([FromQuery] string userId)
        {
            var response = await _serviceManager.UserService.GetByUserId(userId);
            return Ok(response);
        }

        [HttpPost("addUser")]
        public async Task<ActionResult> AddUser(RegisterDto project)
        {
            var response = await _serviceManager.UserService.CreateUser(project);
            return Ok(response);
        }

        [HttpPatch("updateUser")]
        public async Task<ActionResult> UpdateUser([FromQuery] string userId, [FromBody] UpdateUserRequest user)
        {
            var response = await _serviceManager.UserService.UpdateUser(userId, user);
            return Ok(response);
        }

        [HttpDelete("DeleteUser")]
        public async Task<ActionResult> DeleteUser([FromQuery] string userId)
        {
            var response = await _serviceManager.UserService.DeleteUser(userId);
            return Ok(response);
        }

        [HttpPatch("assignTask")]
        public async Task<ActionResult> AssignUser([FromQuery] string userId, [FromBody] AssignTaskRequest request)
        {
            var response = await _serviceManager.UserService.AssignTask(userId, request.Operation, request.TaskId);
            return Ok(response);
        }
    }
}
