using System;

namespace MoneyManagerService.Models.Domain
{
    public interface IIdentifiable<TKey> where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        TKey Id { get; set; }
    }
}