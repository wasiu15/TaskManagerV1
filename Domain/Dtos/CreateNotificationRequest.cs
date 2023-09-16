using Domain;

namespace TaskManager.Domain.Dtos
{
    public class CreateNotificationRequest
    {
        public string TaskId { get; set; }
        public NotificationType Type { get; set; }
        public string Message { get; set; }
    }
}