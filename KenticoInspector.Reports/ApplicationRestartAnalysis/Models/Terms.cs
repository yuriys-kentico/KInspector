using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.ApplicationRestartAnalysis.Models
{
    public class Terms
    {
        public Term InformationSummary { get; set; }

        public TableNamesTerms TableNames { get; set; }
    }

    public class TableNamesTerms
    {
        public Term ApplicationRestartEvents { get; set; }
    }
}