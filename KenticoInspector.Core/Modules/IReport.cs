using KenticoInspector.Core.Models.Results;

namespace KenticoInspector.Core.Modules
{
    public interface IReport : IModule
    {
        ReportResults GetResults();
    }
}