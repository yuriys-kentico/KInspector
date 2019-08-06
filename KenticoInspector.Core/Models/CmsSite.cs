using System;

namespace KenticoInspector.Core.Models
{
    public class CmsSite
    {
        public int SiteId { get; set; }

        public string SiteName { get; set; }

        public Guid SiteGUID { get; set; }

        public string SiteDomainName { get; set; }

        public string SitePresentationURL { get; set; }

        public bool SiteIsContentOnly { get; set; }
    }
}