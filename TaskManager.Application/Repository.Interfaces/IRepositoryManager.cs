using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Application.Repository.Interfaces
{
    public interface IRepositoryManager
    {
        ITaskRepository TaskRepository { get; }
        void Save();
    }
}
