using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Dtos
{
    public class GenericResponse<T>
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public bool IsSuccessful { get; set; }
        public T Data { get; set; }
    }
}
