using System.Collections.Generic;
using System.Xml.Linq;

using KenticoInspector.Core.Constants;

namespace KenticoInspector.Core.Services
{
    public interface ICmsFileService : IService
    {
        IDictionary<string, string> GetResourceStringsFromResx(string instanceRoot, string relativeResxFilePath = DefaultKenticoPaths.PrimaryResxFile);

        XDocument GetXDocument(string instanceRoot, string relativeFilePath);
    }
}