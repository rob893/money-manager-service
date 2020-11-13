using System.Text;
using System;

namespace MoneyManagerService.Models.Domain
{
    public interface IIdentifiable<TKey> where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        TKey Id { get; set; }
    }

    public abstract class IntIdentifiable : IIdentifiable<int>
    {
        public int Id { get; set; }

        public string ConvertIdToBase64()
        {
            return Convert.ToBase64String(BitConverter.GetBytes(Id));
        }

        public static int ConvertBase64StringToIdType(string str)
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

        public static string ConvertBase64StringToIdType(string str)
        {
            try
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(str));
            }
            catch
            {
                throw new ArgumentException($"{str} is not a valid base 64 encoded string.");
            }
        }
    }
}