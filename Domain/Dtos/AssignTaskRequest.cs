using Domain;

namespace TaskManager.Domain.Dtos
{
    public class AssignTaskRequest
    {
        public AddOrDelete Operation { get; set; }
        public string TaskId { get; set; }
    }
}
