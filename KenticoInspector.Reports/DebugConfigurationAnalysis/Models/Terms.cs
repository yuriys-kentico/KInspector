using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.DebugConfigurationAnalysis.Models
{
    public class Terms
    {
        public Term GoodSummary { get; set; }

        public Term WarningSummary { get; set; }

        public Term ErrorSummary { get; set; }

        public TableNamesTerms TableNames { get; set; }

        public WebConfigTerms WebConfig { get; set; }
    }

    public class TableNamesTerms
    {
        public Term ExplicitlyEnabledSettings { get; set; }

        public Term Overview { get; set; }

        public Term WebConfig { get; set; }
    }

    public class WebConfigTerms
    {
        public Term DebugKeyDisplayName { get; set; }

        public Term TraceKeyDisplayName { get; set; }
    }
}