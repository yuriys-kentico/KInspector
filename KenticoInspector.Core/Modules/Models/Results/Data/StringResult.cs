using System.Web;

namespace KenticoInspector.Core.Modules.Models.Results.Data
{
    public class StringResult : Result
    {
        public string String { get; }

        public override bool HasData => !string.IsNullOrWhiteSpace(String);

        internal StringResult(string stringData)
        {
            String = HttpUtility.HtmlEncode(stringData);
        }
    }
}