namespace MoneyManagerService.Models.Settings
{
    public record TaxeeSettings
    {
        public string BaseUrl { get; init; } = default!;
        public string ApiKey { get; init; } = default!;
        public int RequestRetryAttempts { get; init; }
    }
}