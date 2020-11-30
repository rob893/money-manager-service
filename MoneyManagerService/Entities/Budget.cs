using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MoneyManagerService.Entities
{
    public class Budget : IIdentifiable<int>, IOwnedByUser<int>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public List<Expense> Expenses { get; set; } = new List<Expense>();
        [JsonConverter(typeof(StringEnumConverter))]
        public TaxFilingStatus TaxFilingStatus { get; set; } = TaxFilingStatus.Single;
        public TaxLiability TaxLiability { get; set; } = default!;
        public List<Income> Incomes { get; set; } = new List<Income>();
    }
}