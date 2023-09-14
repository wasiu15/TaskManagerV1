using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Dtos
{
    public class LoginResponse
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public string CreatedAt { get; set; }
        public DateTime? TokenGenerationTime { get; set; }
    }
}
