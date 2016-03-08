using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Helpers
{
    public static class SqlDataReaderExtensions
    {

        /// <summary>
        /// Retrieve business object from data transfer object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="initializer"></param>
        /// <returns></returns>
        public static IList<T> RetrieveEntities<T>(DataTable source, Func<DataRow, T> initializer)
        {
            return (from DataRow dataRow in source.Rows select initializer(dataRow)).ToList();
        }


        /// <summary>
        /// Get the column value from datarow. 
        /// If column didn't exists it returns the default value pf type
        /// </summary>
        /// <typeparam name="T">All datatype</typeparam>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static T GetOrdinal<T>(this DataRow dr, string columnName)
        {
            return dr.Table.Columns.Contains(columnName) ? dr[columnName] != null ? dr.Field<T>(columnName) : default(T) : default(T);
        }

        /// <summary>
        /// Disassemble business object to data transfter object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="source"></param>
        /// <param name="initializer"></param>
        /// <returns></returns>
        public static IList<TV> RetrieveDtoEntities<T, TV>(T source, Func<T, TV> initializer)
        {
            return (source.SingleItemAsEnumerable().Select(initializer)).ToList();
        }

        /// <summary>
        /// Tests if the current instance is null and, if so, returns an empty instance of the same type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> e)
        {
            return e ?? Enumerable.Empty<T>();
        }

        // usage: someObject.SingleItemAsEnumerable();
        public static IEnumerable<T> SingleItemAsEnumerable<T>(this T item)
        {
            yield return item;
        }

    }
}
