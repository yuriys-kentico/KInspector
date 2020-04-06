namespace KenticoInspector.Reports.DatabaseTableSizeAnalysis.Data
{
    public class DatabaseTableSize
    {
        public string TableName { get; set; } = null!;

        public int Rows { get; set; }

        public int SizeInMB { get; set; }

        public int BytesPerRow { get; set; }
    }
}