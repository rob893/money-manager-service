using MoneyManagerService.Entities;

namespace MoneyManagerService.Models.DTOs.Income
{
    public record UpdateIncomeDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public IncomeType? IncomeType { get; set; }
        public double? Amount { get; set; }
    }
}