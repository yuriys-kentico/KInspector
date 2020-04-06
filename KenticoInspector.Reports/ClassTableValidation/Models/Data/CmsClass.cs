namespace KenticoInspector.Reports.ClassTableValidation.Models.Data
{
    public class CmsClass
    {
        public int ClassID { get; set; }

        public string ClassDisplayName { get; set; } = null!;

        public string ClassName { get; set; } = null!;

        public string? ClassTableName { get; set; }
    }
}