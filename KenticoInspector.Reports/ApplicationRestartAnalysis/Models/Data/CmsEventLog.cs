﻿using System;

namespace KenticoInspector.Reports.ApplicationRestartAnalysis.Models.Data
{
    public class CmsEventLog
    {
        public int EventID { get; set; }

        public string EventCode { get; set; } = null!;

        public DateTime EventTime { get; set; }

        public string? EventMachineName { get; set; }
    }
}