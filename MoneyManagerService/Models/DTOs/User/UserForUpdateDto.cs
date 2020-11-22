using System.ComponentModel.DataAnnotations;

namespace MoneyManagerService.Models.DTOs
{
    public record UserForUpdateDto
    {
        [MaxLength(255)]
        public string? FirstName { get; init; }
        [MaxLength(255)]
        public string? LastName { get; init; }
    }
}