namespace KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models.Data
{
    public class CmsDocument
    {
        public int DocumentID { get; set; }

        public string DocumentName { get; set; }

        public string DocumentNamePath { get; set; }

        public int DocumentForeignKeyValue { get; set; }

        public int DocumentNodeID { get; set; }
    }
}