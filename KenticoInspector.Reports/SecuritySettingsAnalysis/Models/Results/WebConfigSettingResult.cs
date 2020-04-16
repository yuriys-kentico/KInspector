using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Results
{
    public class WebConfigSettingResult
    {
        public string KeyPath { get; set; }

        public string KeyName { get; set; } = null!;

        public string? KeyValue { get; set; }

        public string RecommendedValue { get; set; } = null!;

        public string RecommendationReason { get; set; } = null!;

        public WebConfigSettingResult(XElement element)
        {
            KeyPath = GetPath(element);
        }

        private string GetPath(XElement element)
        {
            var elementsOnPath = element.AncestorsAndSelf()
                .Reverse()
                .Select(
                    elementOnPath =>
                    {
                        var trimmedElement = new XElement(elementOnPath);
                        trimmedElement.RemoveNodes();

                        return trimmedElement.ToString()
                            .Replace(
                                " />",
                                ">"
                                );
                    }
                    );

            var path = string.Join(
                "/",
                elementsOnPath
                );

            return HttpUtility.HtmlEncode(path);
        }
    }
}