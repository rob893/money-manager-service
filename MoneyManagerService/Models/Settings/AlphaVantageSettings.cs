namespace MoneyManagerService.Models.Settings
{
    public record AlphaVantageSettings
    {
        public string BaseUrl { get; init; } = default!;
        public string ApiKey { get; init; } = default!;
        public int RequestRetryAttempts { get; init; }
    }
}