namespace MoneyManagerService.Models.Responses.Taxee
{
    public record CalculateIncomeTaxResponse
    {
        public TaxJurisdication Annual { get; init; }
        public TaxJurisdication PerPayPeriod { get; init; }
    }
}