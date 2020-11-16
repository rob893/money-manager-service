namespace MoneyManagerService.Models.DTOs
{
    public record LoginForReturnDto
    {
        public string Token { get; init; }
        public string RefreshToken { get; init; }
        public UserForReturnDto User { get; init; }
    }
}