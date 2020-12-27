using System.ComponentModel.DataAnnotations;

namespace MoneyManagerService.Models.DTOs
{
    public record LoginUserDto
    {
        [Required]
        public string Username { get; init; } = default!;
        [Required]
        public string Password { get; init; } = default!;
        [Required]
        public string DeviceId { get; init; } = default!;
    }
}