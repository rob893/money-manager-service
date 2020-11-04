namespace MoneyManagerService.Models.Settings
{
    public class AlphaVantageSettings
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public int RequestRetryAttempts { get; set; }
    }
}