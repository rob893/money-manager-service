using System.ComponentModel.DataAnnotations;

namespace MoneyManagerService.Models.DTOs
{
    public class RefreshTokenDto
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}