using System.ComponentModel.DataAnnotations;

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
