using MoneyManagerService.Models.Domain;

namespace MoneyManagerService.Models.DTOs.Expense
{
    public record ExpenseForUpdateDto
    {
        public string Name { get; init; } = default!;
        public string? Description { get; init; }
        public double? Amount { get; init; }
        public Frequency? Frequency { get; init; }
    }
}