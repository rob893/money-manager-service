using System.Collections.Generic;
using MoneyManagerService.Entities;
using MoneyManagerService.Models.DTOs.Expense;
using MoneyManagerService.Models.DTOs.Income;
using MoneyManagerService.Models.DTOs.TaxLiability;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MoneyManagerService.Models.DTOs.Budget
{
    public record BudgetDto : IIdentifiable<int>, IOwnedByUser<int>
    {
        public int Id { get; init; }
        public int UserId { get; init; }
        public string Name { get; init; } = default!;
        public string? Description { get; init; }
        [JsonConverter(typeof(StringEnumConverter))]
        public TaxFilingStatus TaxFilingStatus { get; init; }
        public TaxLiabilityDto TaxLiability { get; set; } = default!;
        public List<IncomeDto> Incomes { get; init; } = new List<IncomeDto>();
        public List<ExpenseDto> Expenses { get; init; } = new List<ExpenseDto>();
    }
}