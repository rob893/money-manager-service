using System;

namespace MoneyManagerService.Models.Domain
{
    public interface IOwnedByUser<TKey> where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        TKey UserId { get; }
    }
}