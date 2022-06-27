using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MoneyManagerService.Core;
using MoneyManagerService.Entities;
using MoneyManagerService.Models.QueryParameters;

namespace MoneyManagerService.Data.Repositories
{
    public class TagRepository : Repository<Tag, TagQueryParameters>
    {
        public TagRepository(DataContext context) : base(context) { }

        public Task<CursorPagedList<Tag, int>> SearchTagsForUserAsync(int userId, TagQueryParameters searchParams)
        {
            if (searchParams == null)
            {
                throw new ArgumentNullException(nameof(searchParams));
            }

            var query = this.context.Tags
                .Where(tag => tag.UserId == userId);

            if (!string.IsNullOrWhiteSpace(searchParams.NameLike))
            {
                query = query.Where(tag => tag.Name.Contains(searchParams.NameLike));
            }

            return CursorPagedList<Tag, int>.CreateAsync(query, searchParams);
        }

        public Task<List<Tag>> GetTagsForUserAsync(int userId)
        {
            var query = this.context.Tags
                .Where(tag => tag.UserId == userId);

            return query.ToListAsync();
        }

        protected override IQueryable<Tag> AddWhereClauses(IQueryable<Tag> query, TagQueryParameters searchParams)
        {
            if (searchParams == null)
            {
                throw new ArgumentNullException(nameof(searchParams));
            }

            if (!string.IsNullOrWhiteSpace(searchParams.NameLike))
            {
                query = query.Where(tag => tag.Name.Contains(searchParams.NameLike));
            }

            return query;
        }
    }
}