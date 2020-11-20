namespace MoneyManagerService.Models.DTOs
{
    public record LoginForReturnDto
    {
        public string Token { get; init; } = default!;
        public string RefreshToken { get; init; } = default!;
        public UserForReturnDto User { get; init; } = default!;
    }
}