using System.Collections.Generic;

namespace KenticoInspector.Reports.TemplateLayoutAnalysis.Models.Results
{
    public class IdenticalPageTemplateResult
    {
        public string PageTemplateCodeNamesAndIds { get; set; }

        public string PageTemplateLayout { get; set; }

        public IdenticalPageTemplateResult(string pageTemplateLayout, IEnumerable<string> pageTemplateCodeNamesAndIds)
        {
            PageTemplateLayout = pageTemplateLayout;
            PageTemplateCodeNamesAndIds = string.Join(", ", pageTemplateCodeNamesAndIds);
        }
    }
}