using KenticoInspector.Core.Models.Results;

namespace KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models.Results
{
    public class ConsistencyResult
    {
        public Result Data { get; set; } = null!;

        public int Count { get; set; }
    }
}