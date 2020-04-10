using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Instances.Models;
using KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Data;

namespace KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Results
{
    public class TemplateUsageResult : CmsTreeNode
    {
        public string SiteName { get; set; }

        public TemplateUsageResult(CmsTreeNode page, IEnumerable<CmsSite> sites)
        {
            SiteName = sites
                .FirstOrDefault(site => site.SiteId == page.NodeSiteID)
                .SiteName;

            DocumentName = page.DocumentName;
            NodeID = page.NodeID;
            NodeAliasPath = page.NodeAliasPath;
            DocumentCulture = page.DocumentCulture;
            NodeSiteID = page.NodeSiteID;
            DocumentPageTemplateID = page.DocumentPageTemplateID;
        }
    }
}