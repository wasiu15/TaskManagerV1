using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum Priority
    {
        None = 0,
        Low = 1,
        Medium = 2,
        High = 3
    }
    public enum Status
    {
        None = 0,
        Pending = 1,
        In_progress = 2,
        completed = 3
    }
    public enum Type
    {
        Due_date = 1,
        Status_update = 2
    }
}
