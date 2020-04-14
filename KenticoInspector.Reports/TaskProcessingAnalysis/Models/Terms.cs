using KenticoInspector.Core.TokenExpressions.Models;

namespace KenticoInspector.Reports.TaskProcessingAnalysis.Models
{
    public class Terms
    {
        public Summaries Summaries { get; set; } = null!;

        public TableTitles TableTitles { get; set; } = null!;
    }

    public class Summaries
    {
        public Term Warning { get; set; } = null!;

        public Term Good { get; set; } = null!;
    }

    public class TableTitles
    {
        public Term IntegrationBusTasks { get; set; } = null!;

        public Term ScheduledTasks { get; set; } = null!;

        public Term SearchTasks { get; set; } = null!;

        public Term StagingTasks { get; set; } = null!;

        public Term WebFarmTasks { get; set; } = null!;
    }
}