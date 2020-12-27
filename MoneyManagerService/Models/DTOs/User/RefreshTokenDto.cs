using System.ComponentModel.DataAnnotations;

namespace MoneyManagerService.Models.DTOs
{
    public record RefreshTokenDto
    {
        [Required]
        public string Token { get; init; } = default!;
        [Required]
        public string RefreshToken { get; init; } = default!;
        [Required]
        public string DeviceId { get; init; } = default!;
    }
}