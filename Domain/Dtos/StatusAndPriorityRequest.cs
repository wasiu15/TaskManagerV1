using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Dtos
{
    public class StatusAndPriorityRequest
    {
        public Status TaskStatus { get; set; }
        public Priority TaskPriority { get; set; }
    }
}
