using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Core.Modules.Models.Results.Data
{
    public class ReportResultsData : IEnumerable
    {
        private readonly IList<Result> results;

        internal ReportResultsData()
        {
            results = new List<Result>();
        }

        public IEnumerator GetEnumerator() => results.GetEnumerator();

        public static implicit operator ReportResultsData(Result result) => new ReportResultsData
        {
            result
        };

        public T First<T>()
            where T : Result => results
            .OfType<T>()
            .First();

        public bool Add(
            Result result,
            bool addIfNoData = false
            )
        {
            var added = addIfNoData || result.HasData;
            if (added) results.Add(result);

            return added;
        }
    }
}