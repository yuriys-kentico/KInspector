﻿using KenticoInspector.Core;
using KenticoInspector.Core.Models;
using System;
using System.Collections.Generic;

namespace KenticoInspector.Reports
{
    public class TestReport2 : IReport
    {
        public string Codename => "test-report-2";
        public IList<Version> CompatibleVersions => new List<Version> {
            new Version("10.0"),
            new Version("11.0")
        };
        public IList<Version> IncompatibleVersions => new List<Version> {
            new Version("12.0")
        };
        public string LongDescription => $"<p>This is a <em>Long</em> description that can include <a href=\"https://www.kentico.com/\">links to stuff</a> and other HTML 2</p>";
        public string Name => "Test Report 2";
        public string ShortDescription => "This is a short description. 2";
        public IList<string> Tags => new List<string> {
            "Test",
            "Fake",
            "Duplicate"
        };

        public ReportResults GetResults(Guid InstanceGuid)
        {
            return new ReportResults()
            {
                Status = ReportResultsStatus.Information.ToString(),
                Summary = "Test 2 is fake.",
                Type = typeof(string).ToString(),
                Data = "Totally fake 2"
            };
        }
    }
}