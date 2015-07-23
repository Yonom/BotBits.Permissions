using System;
using System.Data;

namespace BotBits.Permissions
{
    internal static class DatabaseExtensions
    {
        public static TResult GetValue<TResult>(
            this DataRow record,
            string name)
        {
            var result = record[name];
            return !result.Equals(DBNull.Value) ? (TResult)result : default(TResult);
        }
    }
}