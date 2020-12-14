using System.ComponentModel.DataAnnotations;
using MoneyManagerService.Entities;

namespace MoneyManagerService.Models.DTOs.Budget
{
    public record CreateExpenseForBudgetDto
    {
        [Required]
        public string Name { get; init; } = default!;
        public string? Description { get; init; }
        [Required]
        public double? Amount { get; init; }
        [Required]
        public Frequency? Frequency { get; init; }
    }
}