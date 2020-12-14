using MoneyManagerService.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MoneyManagerService.Models.DTOs.Income
{
    public record IncomeDto : IIdentifiable<int>
    {
        public int Id { get; init; }
        public int BudgetId { get; init; }
        [JsonConverter(typeof(StringEnumConverter))]
        public IncomeType IncomeType { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public double Amount { get; set; }
    }
}