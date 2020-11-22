using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MoneyManagerService.Entities
{
    public class Income : IIdentifiable<int>
    {
        public int Id { get; set; }
        public int BudgetId { get; set; }
        public Budget Budget { get; set; } = default!;
        [JsonConverter(typeof(StringEnumConverter))]
        public IncomeType IncomeType { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public double Amount { get; set; }
    }
}