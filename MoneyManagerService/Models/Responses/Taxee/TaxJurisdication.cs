namespace MoneyManagerService.Models.Responses.Taxee
{
    public record TaxJurisdication
    {
        public TaxDetails Federal { get; init; } = default!;
        public TaxDetails Fica { get; init; } = default!;
        public TaxDetails State { get; init; } = default!;
    }
}