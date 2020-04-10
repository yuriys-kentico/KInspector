using KenticoInspector.Core.Modules.Models.Results;

namespace KenticoInspector.Core.Modules.Models
{
    public interface IAction : IModule
    {
        ActionResults GetResults(string optionsJson);
    }
}