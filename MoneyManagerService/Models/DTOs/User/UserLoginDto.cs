namespace MoneyManagerService.Models.DTOs
{
    public record UserLoginDto
    {
        public string Token { get; init; } = default!;
        public string RefreshToken { get; init; } = default!;
        public UserDto User { get; init; } = default!;
    }
}