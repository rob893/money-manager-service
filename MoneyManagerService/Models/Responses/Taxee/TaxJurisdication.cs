namespace MoneyManagerService.Models.Responses.Taxee
{
    public record TaxJurisdication
    {
        public TaxDetails Federal { get; init; }
        public TaxDetails Fica { get; init; }
        public TaxDetails State { get; init; }
    }
}