using KenticoInspector.Core.TokenExpressions.Models;

namespace KenticoInspector.Reports.DebugConfigurationAnalysis.Models
{
    public class Terms
    {
        public Term GoodSummary { get; set; } = null!;

        public Term WarningSummary { get; set; } = null!;

        public Term ErrorSummary { get; set; } = null!;

        public TableNamesTerms TableNames { get; set; } = null!;

        public WebConfigTerms WebConfig { get; set; } = null!;
    }

    public class TableNamesTerms
    {
        public Term ExplicitlyEnabledSettings { get; set; } = null!;

        public Term Overview { get; set; } = null!;

        public Term WebConfig { get; set; } = null!;
    }

    public class WebConfigTerms
    {
        public Term DebugKeyDisplayName { get; set; } = null!;

        public Term TraceKeyDisplayName { get; set; } = null!;
    }
}