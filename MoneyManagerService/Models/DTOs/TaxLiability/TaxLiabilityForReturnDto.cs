using MoneyManagerService.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MoneyManagerService.Models.DTOs.TaxLiability
{
    public record TaxLiabilityForReturnDto : IIdentifiable<int>
    {
        public int Id { get; set; }
        public int BudgetId { get; set; }
        public double Federal { get; set; }
        public double Fica { get; set; }
        public double State { get; set; }
    }
}