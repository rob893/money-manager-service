using System;
using System.Collections.Generic;
using System.Linq;
using MoneyManagerService.Core;
using MoneyManagerService.Extensions;
using MoneyManagerService.Models.Domain;

namespace MoneyManagerService.Models.Responses
{
    public class CursorPaginatedResponse<TEntity, TEntityKey>
        where TEntity : class, IIdentifiable<TEntityKey>
        where TEntityKey : IEquatable<TEntityKey>, IComparable<TEntityKey>
    {
        public IEnumerable<Edge<TEntity>> Edges { get; set; }
        public IEnumerable<TEntity> Nodes { get; set; }
        public PageInfo PageInfo { get; set; }
        public int? TotalCount { get; set; }

        private readonly Func<TEntityKey, string> ConvertIdToBase64;


        public CursorPaginatedResponse(IEnumerable<TEntity> items, string startCursor, string endCursor, bool hasNextPage, bool hasPreviousPage, int? totalCount, Func<TEntityKey, string> ConvertIdToBase64)
        {
            this.ConvertIdToBase64 = ConvertIdToBase64;

            SetEdges(items);
            Nodes = items.ToList();
            PageInfo = new PageInfo
            {
                StartCursor = startCursor,
                EndCursor = endCursor,
                HasNextPage = hasNextPage,
                HasPreviousPage = hasPreviousPage
            };
            TotalCount = totalCount;
        }

        public CursorPaginatedResponse(CursorPagedList<TEntity, TEntityKey> items, Func<TEntityKey, string> ConvertIdToBase64)
        {
            this.ConvertIdToBase64 = ConvertIdToBase64;

            SetEdges(items);
            Nodes = items.ToList();
            PageInfo = new PageInfo
            {
                StartCursor = items.StartCursor,
                EndCursor = items.EndCursor,
                HasNextPage = items.HasNextPage,
                HasPreviousPage = items.HasPreviousPage
            };
            TotalCount = items.TotalCount;
        }

        public static CursorPaginatedResponse<TDestination, int> CreateFrom<TSource, TDestination>(CursorPagedList<TSource, int> items, Func<IEnumerable<TSource>, IEnumerable<TDestination>> mappingFunction)
            where TSource : class, IIdentifiable<int>
            where TDestination : class, IIdentifiable<int>
        {
            var mappedItems = mappingFunction(items);

            return new CursorPaginatedResponse<TDestination, int>(mappedItems, items.StartCursor, items.EndCursor, items.HasNextPage, items.HasPreviousPage, items.TotalCount, Id => Convert.ToBase64String(BitConverter.GetBytes(Id)));
        }

        private void SetEdges(IEnumerable<TEntity> items)
        {
            Edges = items.Select(item => new Edge<TEntity>
            {
                Cursor = ConvertIdToBase64(item.Id),
                Node = item
            });
        }
    }

    public class Edge<T>
    {
        public string Cursor { get; set; }
        public T Node { get; set; }
    }

    public class PageInfo
    {
        public string StartCursor { get; set; }
        public string EndCursor { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
}