using MoneyManagerService.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MoneyManagerService.Models.DTOs.Expense
{
    public record ExpenseDto : IIdentifiable<int>
    {
        public int Id { get; init; }
        public int BudgetId { get; init; }
        public string Name { get; init; } = default!;
        public string? Description { get; init; }
        public double Amount { get; init; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Frequency Frequency { get; init; }
    }
}