using KenticoInspector.Core.Modules.Models.Results.Data;

namespace KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models.Results
{
    public class ConsistencyResult
    {
        public Result Data { get; set; } = null!;

        public int Count { get; set; }
    }
}