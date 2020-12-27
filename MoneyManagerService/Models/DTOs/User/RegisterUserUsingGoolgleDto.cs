using System.ComponentModel.DataAnnotations;

namespace MoneyManagerService.Models.DTOs
{
    public record RegisterUserUsingGoolgleDto
    {
        [Required]
        public string IdToken { get; init; } = default!;
        [Required]
        public string UserName { get; init; } = default!;
        [Required]
        public string DeviceId { get; init; } = default!;
    }
}