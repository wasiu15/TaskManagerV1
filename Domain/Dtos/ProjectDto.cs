using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Dtos
{
    public class ProjectDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<TaskDto> AssociatedTasks { get; set; }
    }
}
