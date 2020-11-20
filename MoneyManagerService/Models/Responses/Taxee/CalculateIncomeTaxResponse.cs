namespace MoneyManagerService.Models.Responses.Taxee
{
    public record CalculateIncomeTaxResponse
    {
        public TaxJurisdication Annual { get; init; } = default!;
        public TaxJurisdication? PerPayPeriod { get; init; }
    }
}