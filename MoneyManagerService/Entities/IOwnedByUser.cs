using System;

namespace MoneyManagerService.Entities
{
    public interface IOwnedByUser<TKey> where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        TKey UserId { get; }
    }
}