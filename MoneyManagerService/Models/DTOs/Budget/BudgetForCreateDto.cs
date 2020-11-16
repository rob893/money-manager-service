using System.ComponentModel.DataAnnotations;

namespace MoneyManagerService.Models.DTOs.Budget
{
    public record BudgetForCreateDto
    {
        [Required]
        public string Name { get; init; }
        [Required]
        public int? UserId { get; init; }
        public string Description { get; init; }
    }
}