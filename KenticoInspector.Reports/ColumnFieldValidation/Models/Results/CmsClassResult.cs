namespace KenticoInspector.Reports.ColumnFieldValidation.Models.Results
{
    public class CmsClassResult
    {
        public int ClassID { get; set; }

        public string ClassName { get; set; } = null!;

        public string ClassDisplayName { get; set; } = null!;

        public string? ClassTableName { get; set; }

        public string ClassFieldsNotInTable { get; set; } = null!;
    }
}