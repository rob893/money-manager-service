using System.Text;
using System;

namespace MoneyManagerService.Models.Domain
{
    public interface IIdentifiable<TKey> where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        TKey Id { get; set; }
        TKey ConvertBase64StringToIdType(string str);
        string ConvertIdToBase64();
    }

    public abstract class IntIdentifiable : IIdentifiable<int>
    {
        public int Id { get; set; }

        public string ConvertIdToBase64()
        {
            return Convert.ToBase64String(BitConverter.GetBytes(Id));
        }

        public int ConvertBase64StringToIdType(string str)
        {
            try
            {
                return BitConverter.ToInt32(Convert.FromBase64String(str), 0);
            }
            catch
            {
                throw new ArgumentException($"{str} is not a valid base 64 encoded int32.");
            }
        }
    }

    public abstract class StringIdentifiable : IIdentifiable<string>
    {
        public string Id { get; set; }

        public string ConvertIdToBase64()
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(Id));
        }

        public string ConvertBase64StringToIdType(string str)
        {
            try
            {
                return BitConverter.ToString(Convert.FromBase64String(str), 0);
            }
            catch
            {
                throw new ArgumentException($"{str} is not a valid base 64 encoded string.");
            }
        }
    }
}