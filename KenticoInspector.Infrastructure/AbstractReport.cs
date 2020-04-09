using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Modules;

namespace KenticoInspector.Infrastructure
{
    public abstract class AbstractReport<T> : AbstractModule<T>, IReport where T : new()
    {
        public abstract ReportResults GetResults();
    }
}