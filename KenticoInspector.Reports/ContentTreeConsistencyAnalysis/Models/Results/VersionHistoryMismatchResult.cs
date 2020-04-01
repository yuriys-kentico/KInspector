namespace KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models.Results
{
    public class VersionHistoryMismatchResult
    {
        public int DocumentId { get; set; }

        public string DocumentName { get; set; }

        public string DocumentNamePath { get; set; }

        public string ColumnName { get; set; }

        public string Caption { get; set; }

        public string DocumentValue { get; set; }

        public string VersionHistoryValue { get; set; }
    }
}