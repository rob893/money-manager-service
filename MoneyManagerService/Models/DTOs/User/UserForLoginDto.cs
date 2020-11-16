using System.ComponentModel.DataAnnotations;

namespace MoneyManagerService.Models.DTOs
{
    public record UserForLoginDto
    {
        [Required]
        public string Username { get; init; }
        [Required]
        public string Password { get; init; }
    }
}