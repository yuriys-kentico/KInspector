using System;

namespace KenticoInspector.Reports.TaskProcessingAnalysis.Models.Data
{
    public class CmsWebFarmTask
    {
        public int TaskID { get; set; }

        public string TaskType { get; set; } = null!;

        public string? TaskTarget { get; set; }

        public DateTime TaskCreated { get; set; }

        public string? TaskMachineName { get; set; }
    }
}