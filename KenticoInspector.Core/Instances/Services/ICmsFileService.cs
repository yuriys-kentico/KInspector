using System.Collections.Generic;
using System.Xml.Linq;

namespace KenticoInspector.Core.Instances.Services
{
    public interface ICmsFileService : IService
    {
        IDictionary<string, string> GetResourceStringsFromResx(string instanceRoot, string relativeFilePath);

        XDocument GetXDocument(string instanceRoot, string relativeFilePath);
    }
}