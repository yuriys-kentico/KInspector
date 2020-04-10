using System;

namespace KenticoInspector.Core.Instances.Models
{
    public class CmsSite
    {
        public int SiteId { get; set; }

        public string SiteName { get; set; } = null!;

        public Guid SiteGUID { get; set; }

        public string SiteDomainName { get; set; } = null!;

        public string? SitePresentationURL { get; set; }

        public bool? SiteIsContentOnly { get; set; }
    }
}