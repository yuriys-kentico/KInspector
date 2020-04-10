using System.IO;
using System.Xml.Linq;

namespace KenticoInspector.Modules
{
    public static class FileHelper
    {
        public static XDocument GetXDocumentFromFile(string path)
        {
            var fileText = File.ReadAllText(path);

            return XDocument.Parse(fileText);
        }
    }
}