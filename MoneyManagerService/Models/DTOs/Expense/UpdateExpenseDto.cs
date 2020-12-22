using MoneyManagerService.Entities;

namespace MoneyManagerService.Models.DTOs.Expense
{
    public record UpdateExpenseDto
    {
        public string? Name { get; init; }
        public string? Description { get; init; }
        public double? Amount { get; init; }
        public Frequency? Frequency { get; init; }
    }
}