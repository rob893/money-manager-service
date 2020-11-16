namespace MoneyManagerService.Models.Settings
{
    public record MySQLSettings
    {
        public string DefaultConnection { get; init; }
        public bool EnableSensitiveDataLogging { get; init; }
        public bool EnableDetailedErrors { get; init; }
    }
}