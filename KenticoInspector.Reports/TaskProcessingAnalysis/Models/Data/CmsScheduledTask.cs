using System;

namespace KenticoInspector.Reports.TaskProcessingAnalysis.Models.Data
{
    public class CmsScheduledTask
    {
        public int TaskID { get; set; }

        public string TaskName { get; set; } = null!;

        public bool TaskEnabled { get; set; }

        public DateTime? TaskLastRunTime { get; set; }

        public DateTime? TaskNextRunTime { get; set; }

        public string? TaskServerName { get; set; }

        public bool? TaskRunIndividually { get; set; }

        public bool? TaskUseExternalService { get; set; }
    }
}