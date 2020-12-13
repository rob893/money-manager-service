using System.Collections.Generic;
using MoneyManagerService.Entities;
using MoneyManagerService.Models.DTOs.Expense;
using MoneyManagerService.Models.DTOs.Income;
using MoneyManagerService.Models.DTOs.TaxLiability;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MoneyManagerService.Models.DTOs.Budget
{
    public record BudgetForReturnDto : IIdentifiable<int>, IOwnedByUser<int>
    {
        public int Id { get; init; }
        public int UserId { get; init; }
        public string Name { get; init; } = default!;
        public string? Description { get; init; }
        [JsonConverter(typeof(StringEnumConverter))]
        public TaxFilingStatus TaxFilingStatus { get; init; }
        public TaxLiabilityForReturnDto TaxLiability { get; set; } = default!;
        public List<IncomeForReturnDto> Incomes { get; init; } = new List<IncomeForReturnDto>();
        public List<ExpenseForReturnDto> Expenses { get; init; } = new List<ExpenseForReturnDto>();
    }
}