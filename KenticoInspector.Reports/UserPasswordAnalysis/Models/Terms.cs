using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.UserPasswordAnalysis.Models
{
    public class Terms
    {
        public Term ErrorSummary { get; set; } = null!;

        public Term GoodSummary { get; set; } = null!;

        public TableTitlesTerms TableTitles { get; set; } = null!;
    }

    public class TableTitlesTerms
    {
        public Term EmptyPasswords { get; set; } = null!;

        public Term PlaintextPasswords { get; set; } = null!;
    }
}