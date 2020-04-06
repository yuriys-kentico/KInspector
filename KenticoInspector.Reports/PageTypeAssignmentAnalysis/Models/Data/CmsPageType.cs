namespace KenticoInspector.Reports.PageTypeAssignmentAnalysis.Models.Data
{
    public class CmsPageType
    {
        public int ClassID { get; set; }

        public string ClassDisplayName { get; set; } = null!;

        public string ClassName { get; set; } = null!;

        public int NodeClassID { get; set; }

        public int NodeSiteID { get; set; }
    }
}