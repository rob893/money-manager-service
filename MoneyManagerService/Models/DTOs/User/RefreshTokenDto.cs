using System.ComponentModel.DataAnnotations;

namespace MoneyManagerService.Models.DTOs
{
    public record RefreshTokenDto
    {
        [Required]
        public string Token { get; init; }
        [Required]
        public string RefreshToken { get; init; }
    }
}