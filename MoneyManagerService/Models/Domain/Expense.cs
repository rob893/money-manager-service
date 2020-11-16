using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MoneyManagerService.Models.Domain
{
    public class Expense : IIdentifiable<int>
    {
        public int Id { get; set; }
        public int BudgetId { get; set; }
        public Budget Budget { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Frequency Frequency { get; set; }
    }
}