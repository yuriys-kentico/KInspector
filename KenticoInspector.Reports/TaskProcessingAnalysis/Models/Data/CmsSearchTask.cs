using System;

namespace KenticoInspector.Reports.TaskProcessingAnalysis.Models.Data
{
    public class CmsSearchTask
    {
        public int SearchTaskID { get; set; }

        public string SearchTaskType { get; set; } = null!;

        public string? SearchTaskObjectType { get; set; }

        public string? SearchTaskServerName { get; set; }

        public DateTime SearchTaskCreated { get; set; }

        public int? SearchTaskRelatedObjectID { get; set; }
    }
}