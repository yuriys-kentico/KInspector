using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Services.Interfaces;

namespace KenticoInspector.Core.Helpers
{
    public class CmsFileService : ICmsFileService
    {
        public IDictionary<string, string> GetResourceStringsFromResx(string instanceRoot, string relativeResxFilePath = DefaultKenticoPaths.PrimaryResxFile)
        {
            var resourceXml = GetXDocument(instanceRoot, relativeResxFilePath);

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