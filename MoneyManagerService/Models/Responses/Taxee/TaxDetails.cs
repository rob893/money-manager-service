using Newtonsoft.Json;

namespace MoneyManagerService.Models.Responses.Taxee
{
    public record TaxDetails
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double? Amount { get; init; }
    }
}