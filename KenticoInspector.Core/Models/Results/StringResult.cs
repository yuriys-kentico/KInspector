﻿using System.Web;

namespace KenticoInspector.Core.Models.Results
{
    public class StringResult : Result
    {
        public string String { get; }

        public override bool HasData => !string.IsNullOrWhiteSpace(String);

        internal StringResult(string stringData) => String = HttpUtility.HtmlEncode(stringData);
    }
}