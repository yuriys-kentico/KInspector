using KenticoInspector.Core.Modules.Models;
using KenticoInspector.Core.Modules.Models.Results;

namespace KenticoInspector.Modules
{
    public abstract class AbstractReport<T> : AbstractModule<T>, IReport where T : new()
    {
        public abstract ReportResults GetResults();
    }
}