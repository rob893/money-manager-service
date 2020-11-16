namespace MoneyManagerService.Models.Settings
{
    public record AlphaVantageSettings
    {
        public string BaseUrl { get; init; }
        public string ApiKey { get; init; }
        public int RequestRetryAttempts { get; init; }
    }
}