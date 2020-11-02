using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoneyManagerService.Extensions;
using MoneyManagerService.Models.Domain;
using MoneyManagerService.Models.QueryParameters;

namespace MoneyManagerService.Core
{
    public class CursorPagedList<TEntity, TEntityKey> : List<TEntity>
        where TEntity : class, IIdentifiable<TEntityKey>, new()
        where TEntityKey : IEquatable<TEntityKey>, IComparable<TEntityKey>
    {
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public string StartCursor { get; set; }
        public string EndCursor { get; set; }
        public int? TotalCount { get; set; }


        public CursorPagedList(IEnumerable<TEntity> items, bool hasNextPage, bool hasPreviousPage, string startCursor, string endCursor, int? totalCount)
        {
            HasNextPage = hasNextPage;
            HasPreviousPage = hasPreviousPage;
            StartCursor = startCursor;
            EndCursor = endCursor;
            TotalCount = totalCount;
            AddRange(items);
        }

        public static async Task<CursorPagedList<TEntity, TEntityKey>> CreateAsync(IQueryable<TEntity> source, int? first, string after, int? last, string before, bool includeTotal = false)
        {
            if (first != null && last != null)
            {
                throw new ArgumentException("Passing both `first` and `last` to paginate is not supported.");
            }

            int? totalCount = null;

            if (includeTotal)
            {
                totalCount = await source.CountAsync();
            }

            if (first == null && last == null)
            {
                var items = await source.OrderBy(item => item.Id).ToListAsync();

                var startCursor = items.FirstOrDefault()?.ConvertIdToBase64();
                var endCursor = items.LastOrDefault()?.ConvertIdToBase64();

                return new CursorPagedList<TEntity, TEntityKey>(items, false, false, startCursor, endCursor, totalCount);
            }

            if (first != null)
            {
                if (first.Value < 0)
                {
                    throw new ArgumentException("first cannot be less than 0.");
                }

                var afterId = after == null ? int.MinValue : after.ConvertToInt32FromBase64();
                var beforeId = before == null ? int.MaxValue : before.ConvertToInt32FromBase64();

                var items = await source.Where(item => item.Id > afterId && item.Id < beforeId)
                    .OrderBy(item => item.Id)
                    .Take(first.Value + 1).ToListAsync();

                var hasNextPage = items.Count >= first.Value + 1;
                var hasPreviousPage = after != null;

                if (items.Count >= first.Value + 1)
                {
                    items.RemoveAt(items.Count - 1);
                }

                var startCursor = items.FirstOrDefault()?.Id.ConvertInt32ToBase64();
                var endCursor = items.LastOrDefault()?.Id.ConvertInt32ToBase64();

                return new CursorPagedList<TEntity, TEntityKey>(items, hasNextPage, hasPreviousPage, startCursor, endCursor, totalCount);
            }

            if (last != null)
            {
                if (last.Value < 0)
                {
                    throw new ArgumentException("last cannot be less than 0.");
                }

                if (after != null)
                {
                    var afterId = new TEntity().ConvertBase64StringToIdType(after);
                    source = source.Where(item => item.Id.CompareTo(afterId) > 0);
                }

                // var afterId = after == null ? int.MinValue : after.ConvertToInt32FromBase64();
                var beforeId = before == null ? int.MaxValue : before.ConvertToInt32FromBase64();

                var items = await source.Where(item => item.Id > afterId && item.Id < beforeId)
                    .OrderByDescending(item => item.Id)
                    .Take(last.Value + 1).ToListAsync();

                var hasNextPage = before != null;
                var hasPreviousPage = items.Count >= last.Value + 1;

                if (items.Count >= last.Value + 1)
                {
                    items.RemoveAt(items.Count - 1);
                }

                items.Reverse();

                var startCursor = items.FirstOrDefault()?.Id.ConvertInt32ToBase64();
                var endCursor = items.LastOrDefault()?.Id.ConvertInt32ToBase64();

                return new CursorPagedList<TEntity, TEntityKey>(items, hasNextPage, hasPreviousPage, startCursor, endCursor, totalCount);
            }

            throw new Exception("Error creating cursor paged list.");
        }

        public static Task<CursorPagedList<TEntity, TEntityKey>> CreateAsync(IQueryable<TEntity> source, CursorPaginationParameters searchParams)
        {
            return CreateAsync(source, searchParams.First, searchParams.After, searchParams.Last, searchParams.Before, searchParams.IncludeTotal);
        }
    }
}