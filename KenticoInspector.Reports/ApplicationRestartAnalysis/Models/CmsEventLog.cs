using System;

namespace KenticoInspector.Reports.ApplicationRestartAnalysis.Models
{
    public class CmsEventLog
    {
        public string EventCode { get; set; }

        public DateTime EventTime { get; set; }

        public string EventMachineName { get; set; }
    }
}