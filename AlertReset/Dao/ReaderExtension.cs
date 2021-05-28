using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace AlertReset.Dao
{
    public static class ReaderExtension
    {
        public static T GetDBValue<T>(this IDataReader reader, string columnName)
        {
            var iPosicion = reader.GetOrdinal(columnName);

            if (reader.IsDBNull(iPosicion))
                return default(T);
            else
                return (T)reader[columnName];
        }
    }
}
