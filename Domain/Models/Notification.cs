namespace Domain.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public string TaskId { get; set; }
        public Type Type { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
    }
}
