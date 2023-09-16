using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Service.Interfaces;
using TaskManager.Domain.Dtos;

namespace TaskManager.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class NotificationsController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public NotificationsController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }


        [HttpGet("getAllNotifications")]
        public async Task<ActionResult> GetAll()
        {
            var response = await _serviceManager.NotificationService.GetAllNotifications();
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("getNotificationById")]
        public async Task<ActionResult> GetNotificationById([FromQuery] string notificationId)
        {
            var response = await _serviceManager.NotificationService.GetByNotificationId(notificationId);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPost("addNotification")]
        public async Task<ActionResult> AddNotification(CreateNotificationRequest notification)
        {
            var response = await _serviceManager.NotificationService.CreateNotification(notification);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpPatch("ReadOrUnread")]
        public async Task<ActionResult> UpdateNotification([FromQuery] string notificationId)
        {
            var response = await _serviceManager.NotificationService.ReadOrUnread(notificationId);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpDelete("DeleteUser")]
        public async Task<ActionResult> DeleteNotification([FromQuery] string notificationId)
        {
            var response = await _serviceManager.NotificationService.DeleteNotification(notificationId);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

    }
}
