namespace MoneyManagerService.Models.Settings
{
    public record SwaggerSettings
    {
        public SwaggerAuthSettings AuthSettings { get; init; }
        public bool Enabled { get; init; }
    }
}