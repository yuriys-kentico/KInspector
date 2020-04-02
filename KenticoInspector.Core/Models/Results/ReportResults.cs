using KenticoInspector.Core.Constants;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KenticoInspector.Core.Models.Results
{
    public class ReportResults
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ResultsStatus Status { get; set; }

        public string Summary { get; set; }

        public ReportResultsData Data { get; set; }

        public ReportResults(ResultsStatus resultsStatus = ResultsStatus.Information)
        {
            Status = resultsStatus;
            Data = new ReportResultsData();
        }
    }
}