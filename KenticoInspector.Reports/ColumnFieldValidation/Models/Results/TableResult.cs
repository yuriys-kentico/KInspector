namespace KenticoInspector.Reports.ColumnFieldValidation.Models.Results
{
    public class TableResult
    {
        public string TableName { get; set; } = null!;

        public string TableColumnsNotInClass { get; set; } = null!;
    }
}