namespace KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models.Data
{
    public class CmsClassField
    {
        public string? Caption { get; set; }

        public string Column { get; set; } = null!;

        public string ColumnType { get; set; } = null!;

        public string? DefaultValue { get; set; }

        public bool IsIdColumn { get; set; }
    }
}