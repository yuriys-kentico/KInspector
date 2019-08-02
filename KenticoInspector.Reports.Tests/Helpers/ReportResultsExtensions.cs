using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.Tests.Helpers
{
    internal static class ReportResultsExtensions
    {
        public static TResult GetAnonymousTableResult<TResult>(this ReportResults results, string resultName)
        {
            return results
                .Data
                .GetType()
                .GetProperty(resultName)
                .GetValue(results.Data);
        }
    }
}