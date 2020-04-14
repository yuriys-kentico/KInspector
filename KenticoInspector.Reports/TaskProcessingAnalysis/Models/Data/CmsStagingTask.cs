using System;

namespace KenticoInspector.Reports.TaskProcessingAnalysis.Models.Data
{
    public class CmsStagingTask
    {
        public int TaskID { get; set; }

        public string TaskTitle { get; set; } = null!;

        public string? TaskServers { get; set; }

        public DateTime TaskTime { get; set; }

        public int? TaskSiteID { get; set; }

        public string? TaskObjectType { get; set; }

        public int? TaskObjectID { get; set; }

        public int? TaskNodeID { get; set; }

        public int? TaskDocumentID { get; set; }

        public string? TaskNodeAliasPath { get; set; }
    }
}