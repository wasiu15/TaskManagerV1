using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Dtos
{
    public class RefreshTokenDto
    {
        [Required(ErrorMessage = "Refresh token required")]
        public string RefreshToken { get; set; }
        [Required(ErrorMessage = "UserId is required")]
        public string UserId { get; set; }
    }
}
