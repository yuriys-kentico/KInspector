﻿using System;

namespace KenticoInspector.Reports.ApplicationRestartAnalysis.Models
{
    public class ApplicationRestartEvent
    {
        public string EventCode { get; set; }

        public DateTime EventTime { get; set; }

        public string EventMachineName { get; set; }
    }
}