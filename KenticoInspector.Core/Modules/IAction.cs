using KenticoInspector.Core.Models.Results;

namespace KenticoInspector.Core.Modules
{
    public interface IAction : IModule
    {
        ActionResults GetResults(string optionsJson);
    }
}