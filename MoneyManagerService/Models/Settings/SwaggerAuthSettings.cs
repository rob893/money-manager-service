namespace MoneyManagerService.Models.Settings
{
    public record SwaggerAuthSettings
    {
        public string Username { get; init; }
        public string Password { get; init; }
        public bool RequireAuth { get; init; }
    }
}