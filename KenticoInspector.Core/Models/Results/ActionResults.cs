using KenticoInspector.Core.Constants;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KenticoInspector.Core.Models.Results
{
    public class ActionResults
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ResultsStatus Status { get; set; }

        public string Summary { get; set; }
    }
}