using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Models;
using KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Data;

namespace KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Results
{
    public class TemplateUsageResult : CmsTreeNode
    {
        public string SiteName { get; }

        public TemplateUsageResult(CmsTreeNode page, IEnumerable<Site> sites)
        {
            SiteName = sites.FirstOrDefault(site => site.Id == page.NodeSiteID).Name;
            DocumentName = page.DocumentName;
            NodeID = page.NodeID;
            NodeAliasPath = page.NodeAliasPath;
            DocumentCulture = page.DocumentCulture;
            NodeSiteID = page.NodeSiteID;
            DocumentPageTemplateID = page.DocumentPageTemplateID;
        }
    }
}