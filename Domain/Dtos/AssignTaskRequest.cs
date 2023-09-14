﻿using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Dtos
{
    public class AssignTaskRequest
    {
        public AddOrDelete Operation { get; set; }
        public string TaskId { get; set; }
    }
}
