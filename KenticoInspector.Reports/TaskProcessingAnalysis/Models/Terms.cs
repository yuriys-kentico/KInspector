using KenticoInspector.Core.TokenExpressions.Models;

namespace KenticoInspector.Reports.TaskProcessingAnalysis.Models
{
    public class Terms
    {
        public Term GoodSummary { get; set; } = null!;

        public Term WarningSummary { get; set; } = null!;

        public Term CountIntegrationBusTask { get; set; } = null!;

        public Term CountScheduledTask { get; set; } = null!;

        public Term CountSearchTask { get; set; } = null!;

        public Term CountStagingTask { get; set; } = null!;

        public Term CountWebFarmTask { get; set; } = null!;
    }
}