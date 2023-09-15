using Domain;

namespace TaskManager.Domain.Dtos
{
    public class StatusAndPriorityRequest
    {
        public Status TaskStatus { get; set; }
        public Priority TaskPriority { get; set; }
    }
}
