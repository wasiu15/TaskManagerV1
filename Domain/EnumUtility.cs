using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum Priority
    {
        Low,
        Medium,
        High
    }
    public enum Status
    {
        Pending,
        In_progress,
        Completed
    }
    public enum Type
    {
        Due_Date,
        Status_Update
    }
}
