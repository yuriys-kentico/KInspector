using KenticoInspector.Core.TokenExpressions.Models;

namespace KenticoInspector.Reports.DatabaseConsistencyCheck.Models
{
    public class Terms
    {
        public Term ErrorSummary { get; set; } = null!;

        public Term GoodSummary { get; set; } = null!;
    }
}