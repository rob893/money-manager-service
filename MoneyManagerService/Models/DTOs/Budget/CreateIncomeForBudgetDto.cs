using System.ComponentModel.DataAnnotations;
using MoneyManagerService.Entities;

namespace MoneyManagerService.Models.DTOs.Budget
{
    public record CreateIncomeForBudgetDto
    {
        [Required]
        public IncomeType? IncomeType { get; set; }
        [Required]
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        [Required]
        public double? Amount { get; set; }
    }
}