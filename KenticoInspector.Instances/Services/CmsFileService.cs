using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using KenticoInspector.Core.Instances.Services;

namespace KenticoInspector.Instances.Services
{
    public class CmsFileService : ICmsFileService
    {
        public IDictionary<string, string> GetResourceStringsFromResx(string instanceRoot, string relativeFilePath)
        {
            var resourceXml = GetXDocument(instanceRoot, relativeFilePath);

            var resourceStringNodes = resourceXml
                .Descendants("data")
                .ToDictionary(
                    element => element.Attribute("name").Value.ToLowerInvariant(),
                    element => element.Element("value").Value
                );

            return resourceStringNodes;
        }

        public XDocument GetXDocument(string instanceRoot, string relativeFilePath)
        {
            return XDocument.Load(instanceRoot + relativeFilePath);
        }
    }
}