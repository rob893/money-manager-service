using System.ComponentModel.DataAnnotations;
using MoneyManagerService.Models.Domain;

namespace MoneyManagerService.Models.DTOs.Expense
{
    public record ExpenseForCreateDto
    {
        [Required]
        public int? BudgetId { get; init; }
        [Required]
        public string Name { get; init; }
        public string Description { get; init; }
        [Required]
        public double? Amount { get; init; }
        [Required]
        public Frequency? Frequency { get; init; }
    }
}