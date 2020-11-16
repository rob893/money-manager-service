namespace MoneyManagerService.Models.Settings
{
    public record TaxeeSettings
    {
        public string BaseUrl { get; init; }
        public string ApiKey { get; init; }
        public int RequestRetryAttempts { get; init; }
    }
}