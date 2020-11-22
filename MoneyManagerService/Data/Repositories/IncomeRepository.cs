using MoneyManagerService.Entities;
using MoneyManagerService.Models.QueryParameters;

namespace MoneyManagerService.Data.Repositories
{
    public class IncomeRepository : Repository<Income, CursorPaginationParameters>
    {
        public IncomeRepository(DataContext context) : base(context) { }
    }
}