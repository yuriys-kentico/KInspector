using System.Collections.Generic;

namespace KenticoInspector.Core.Models.Results
{
    public static class ResultExtensions
    {
        public static TableResult<T> AsResult<T>(this IEnumerable<T> rows) where T : notnull
        {
            return new TableResult<T>(rows);
        }
    }
}