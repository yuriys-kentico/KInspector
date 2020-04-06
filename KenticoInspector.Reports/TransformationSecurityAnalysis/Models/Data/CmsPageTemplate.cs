using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

using Newtonsoft.Json;

namespace KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Data
{
    /// <summary>
    /// The page template contains its web parts configuration in <see cref="PageTemplateWebParts"/>. This configuration contains references to transformation code names.
    /// </summary>

    [DebuggerDisplay("{PageTemplateCodeName} {PageTemplateDisplayName}")]
    public class CmsPageTemplate
    {
        private IEnumerable<WebPart>? webParts;

        public int PageTemplateID { get; set; }

        public string PageTemplateCodeName { get; set; } = null!;

        public string PageTemplateDisplayName { get; set; } = null!;

        [JsonIgnore]
        public XDocument? PageTemplateWebParts { get; set; }

        [JsonIgnore]
        public IEnumerable<CmsTreeNode> TreeNodes { get; set; } = null!;

        [JsonIgnore]
        public IEnumerable<WebPart>? WebParts
        {
            get
            {
                return webParts ?? (webParts = PageTemplateWebParts?
                   .Descendants("webpart")
                   .Select(webPartXml => new WebPart(webPartXml))
                   .ToList());
            }

            private set => webParts = value;
        }

        public void RemoveWebPartsWithNoProperties()
        {
            WebParts = WebParts
                    .Where(webPart => webPart
                        .Properties
                        .Any()
                    );
        }
    }
}