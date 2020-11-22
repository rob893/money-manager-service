namespace MoneyManagerService.Models.Domain
{
    public class TaxLiability : IIdentifiable<int>
    {
        public int Id { get; set; }
        public int BudgetId { get; set; }
        public Budget Budget { get; set; } = default!;
        public double Federal { get; set; }
        public double Fica { get; set; }
        public double State { get; set; }
    }
}