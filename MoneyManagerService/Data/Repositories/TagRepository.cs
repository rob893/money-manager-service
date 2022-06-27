using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoneyManagerService.Core;
using MoneyManagerService.Entities;
using MoneyManagerService.Models.QueryParameters;

namespace MoneyManagerService.Data.Repositories
{
    public class TagRepository : Repository<Tag, CursorPaginationParameters>
    {
        public TagRepository(DataContext context) : base(context) { }

        public Task<CursorPagedList<Tag, int>> SearchTagsForUserAsync(int userId, CursorPaginationParameters searchParams)
        {
            var query = this.context.Tags
                .Where(tag => tag.UserId == userId);

            return CursorPagedList<Tag, int>.CreateAsync(query, searchParams);
        }

        public Task<List<Tag>> GetTagsForUserAsync(int userId)
        {
            var query = this.context.Tags
                .Where(tag => tag.UserId == userId);

            return query.ToListAsync();
        }
    }
}