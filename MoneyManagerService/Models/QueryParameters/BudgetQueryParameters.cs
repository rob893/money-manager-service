using System.Collections.Generic;

namespace MoneyManagerService.Models.QueryParameters
{
    public record BudgetQueryParameters : CursorPaginationParameters
    {
        public List<int> UserIds { get; init; } = new List<int>();
    }
}