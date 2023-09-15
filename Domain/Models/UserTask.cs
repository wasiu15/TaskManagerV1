using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    //  I NAMED THIS USERTASK AND NOT JUST TASK BECAUSE TASK IS A RESERVED WORD
    public partial class UserTask
    {
        [Key]
        public string TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public Priority Priority { get; set; }
        public Status Status { get; set; }
        public string ProjectId { get; set; }
        public string UserId { get; set; }
    }
}
