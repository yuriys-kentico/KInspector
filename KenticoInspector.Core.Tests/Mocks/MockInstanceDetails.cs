using System;
using System.Collections.Generic;

using KenticoInspector.Core.Instances.Models;

namespace KenticoInspector.Core.Tests.Mocks
{
    internal static class MockInstanceDetails
    {
        internal static InstanceDetails Kentico9 = new InstanceDetails
        {
            AdministrationVersion = new Version("9.0"),
            DatabaseVersion = new Version("9.0"),
            Sites = new List<CmsSite>
            {
                new CmsSite { SiteDomainName = "kentico9.com" }
            }
        };

        internal static InstanceDetails Kentico10 = new InstanceDetails
        {
            AdministrationVersion = new Version("10.0"),
            DatabaseVersion = new Version("10.0"),
            Sites = new List<CmsSite>
            {
                new CmsSite { SiteDomainName = "kentico10.com" }
            }
        };

        internal static InstanceDetails Kentico11 = new InstanceDetails
        {
            AdministrationVersion = new Version("11.0"),
            DatabaseVersion = new Version("11.0"),
            Sites = new List<CmsSite>
            {
                new CmsSite { SiteDomainName = "kentico11.com" }
            }
        };

        internal static InstanceDetails Kentico12 = new InstanceDetails
        {
            AdministrationVersion = new Version("12.0"),
            DatabaseVersion = new Version("12.0"),
            Sites = new List<CmsSite>
            {
                new CmsSite { SiteDomainName = "kentico12.com" }
            }
        };

        internal static InstanceDetails Get(int majorVersion, Instance instance)
        {
            InstanceDetails? instanceDetails = null;

            switch (majorVersion)
            {
                case 9:
                    instanceDetails = Kentico9;
                    break;

                case 10:
                    instanceDetails = Kentico10;
                    break;

                case 11:
                    instanceDetails = Kentico11;
                    break;

                case 12:
                    instanceDetails = Kentico12;
                    break;
            }

            if (instanceDetails != null)
            {
                instanceDetails.Guid = instance.Guid;
            }

            return instanceDetails ?? throw new Exception($"Version '{majorVersion}' not supported.");
        }
    }
}