using KenticoInspector.Core.Modules.Models.Results;

namespace KenticoInspector.Core.Modules.Models
{
    public interface IReport : IModule
    {
        ReportResults GetResults();
    }
}