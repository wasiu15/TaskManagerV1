using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Project
    {
        [Key]
        public Guid ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<UserTask> Tasks { get; set; }
    }
}
