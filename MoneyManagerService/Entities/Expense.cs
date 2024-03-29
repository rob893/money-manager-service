using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MoneyManagerService.Entities
{
    public class Expense : IIdentifiable<int>
    {
        public int Id { get; set; }
        public int BudgetId { get; set; }
        public Budget Budget { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public double Amount { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Frequency Frequency { get; set; }
        public List<Tag> Tags { get; set; } = new();
    }
}