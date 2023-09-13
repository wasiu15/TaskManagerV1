using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    //  I NAMED THIS USERTASK AND NOT JUST TASK BECAUSE TASK IS A RESERVED WORD
    public class UserTask
    {
        [Key]
        public Guid TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateOnly DueDate { get; set; }
        public Priority Priority { get; set; }
        public Status Status { get; set; }
    }
}
