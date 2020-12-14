using System.ComponentModel.DataAnnotations;
using MoneyManagerService.Entities;

namespace MoneyManagerService.Models.DTOs.Budget
{
    public record CreateBudgetDto
    {
        [Required]
        public string Name { get; init; } = default!;
        [Required]
        public int? UserId { get; init; }
        public string? Description { get; init; }
        public TaxFilingStatus? TaxFilingStatus { get; init; }
    }
}