using System.ComponentModel.DataAnnotations;

namespace TaskManager.Domain.Models
{
    public class Project
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<UserTask> UserTasks { get; set; }
    }
}
        