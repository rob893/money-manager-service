using MoneyManagerService.Entities;
using MoneyManagerService.Models.QueryParameters;

namespace MoneyManagerService.Data.Repositories
{
    public class TaxLiabilityRepository : Repository<TaxLiability, CursorPaginationParameters>
    {
        public TaxLiabilityRepository(DataContext context) : base(context) { }
    }
}