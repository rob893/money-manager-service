namespace MoneyManagerService.Models.Settings
{
    public class MySQLSettings
    {
        public string DefaultConnection { get; set; }
        public bool EnableSensitiveDataLogging { get; set; }
        public bool EnableDetailedErrors { get; set; }
    }
}