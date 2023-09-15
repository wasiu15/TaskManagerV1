using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Project
    {
        [Key]
        public string ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<UserTask> UserTasks { get; set; }
    }
}
        