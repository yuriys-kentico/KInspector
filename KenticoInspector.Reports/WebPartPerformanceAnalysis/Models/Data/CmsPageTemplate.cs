using System.Xml.Linq;
using Newtonsoft.Json;

namespace KenticoInspector.Reports.WebPartPerformanceAnalysis.Models.Data
{
    public class CmsPageTemplate
    {
        public string PageTemplateCodeName { get; set; }

        public string PageTemplateDisplayName { get; set; }

        public int PageTemplateID { get; set; }

        [JsonIgnore]
        public XDocument PageTemplateWebParts { get; set; }
    }
}