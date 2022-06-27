namespace MoneyManagerService.Models.QueryParameters
{
    public record TagQueryParameters : CursorPaginationParameters
    {
        public string? NameLike { get; init; }
    }
}