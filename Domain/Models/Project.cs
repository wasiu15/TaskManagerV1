using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> TaskIds { get; set; }
    }
}
