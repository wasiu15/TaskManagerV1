﻿using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Application.Service.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<UserTask>> GetTasksAsync();
    }
}