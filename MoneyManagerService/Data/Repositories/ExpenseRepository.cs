using MoneyManagerService.Models.Domain;
using MoneyManagerService.Models.QueryParameters;

namespace MoneyManagerService.Data.Repositories
{
    public class ExpenseRepository : Repository<Expense, CursorPaginationParameters>
    {
        public ExpenseRepository(DataContext context) : base(context) { }
    }
}