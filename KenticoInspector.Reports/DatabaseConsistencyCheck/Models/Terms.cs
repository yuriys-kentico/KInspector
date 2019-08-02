using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.DatabaseConsistencyCheck.Models
{
    public class Terms
    {
        public Term ErrorSummary { get; set; }

        public Term GoodSummary { get; set; }
    }
}