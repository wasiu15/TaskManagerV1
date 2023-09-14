using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Service.Interfaces;
using TaskManager.Domain.Dtos;

namespace TaskManager.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            return Ok(response);
        }

        [HttpGet("getNotificationById")]
        public async Task<ActionResult> GetNotificationById([FromQuery] string notificationId)
        {
            var response = await _serviceManager.NotificationService.GetByNotificationId(notificationId);
            return Ok(response);
        }

        [HttpPost("addNotification")]
        public async Task<ActionResult> AddNotification(CreateNotificationRequest notification)
        {
            var response = await _serviceManager.NotificationService.CreateNotification(notification);
            return Ok(response);
        }

        [HttpPatch("ReadOrUnread")]
        public async Task<ActionResult> UpdateNotification([FromQuery] string notificationId)
        {
            var response = await _serviceManager.NotificationService.ReadOrUnread(notificationId);
            return Ok(response);
        }

        [HttpDelete("DeleteUser")]
        public async Task<ActionResult> DeleteNotification([FromQuery] string notificationId)
        {
            var response = await _serviceManager.NotificationService.DeleteNotification(notificationId);
            return Ok(response);
        }

    }
}
