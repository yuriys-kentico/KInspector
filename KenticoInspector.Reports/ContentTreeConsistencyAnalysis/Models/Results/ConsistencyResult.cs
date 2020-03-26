using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models.Results;

namespace KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models.Results
{
    public class ConsistencyResult
    {
        public ReportResultsStatus Status { get; set; }

        public string TableName { get; set; }

        public Result Data { get; set; }

        public int Count { get; set; }

        public ConsistencyResult(ReportResultsStatus status, string tableName, Result data, int count)
        {
            Status = status;
            TableName = tableName;
            Data = data;
            Count = count;
        }
    }
}