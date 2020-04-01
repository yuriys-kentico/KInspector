using System.Collections.Generic;

namespace KenticoInspector.Reports.TemplateLayoutAnalysis.Models.Results
{
    public class IdenticalPageTemplateResult
    {
        public string PageTemplateCodenamesAndIds { get; set; }

        public string PageTemplateLayout { get; set; }

        public IdenticalPageTemplateResult(string pageTemplateLayout, IEnumerable<string> pageTemplateCodenamesAndIds)
        {
            PageTemplateLayout = pageTemplateLayout;
            PageTemplateCodenamesAndIds = string.Join(", ", pageTemplateCodenamesAndIds);
        }
    }
}