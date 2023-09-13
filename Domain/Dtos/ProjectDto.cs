using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Dtos
{
    public class ProjectDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateOnly DueDate { get; set; }
    }
}
