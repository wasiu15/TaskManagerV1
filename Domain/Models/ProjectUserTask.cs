using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Models
{
    public class ProjectUserTask
    {
        public string ProjectId { get; set; }
        public Project Project { get; set; }

        public string UserTaskId { get; set; }
        public UserTask UserTask { get; set; }
    }
}
