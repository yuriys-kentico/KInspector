using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Modules;
using KenticoInspector.Core.Services.Interfaces;

using Newtonsoft.Json;

namespace KenticoInspector.Core
{
    public abstract class AbstractAction<TTerms, TOptions>
        : AbstractModule<TTerms>, IAction
        where TTerms : new()
        where TOptions : new()
    {
        protected AbstractAction(IModuleMetadataService moduleMetadataService) : base(moduleMetadataService)
        {
        }

        public ActionResults Execute(string optionsJson)
        {
            TOptions options;

            try
            {
                options = JsonConvert.DeserializeObject<TOptions>(optionsJson);
            }
            catch
            {
                return GetInvalidOptionsResult();
            }

            return Execute(options);
        }

        public abstract ActionResults Execute(TOptions options);

        public abstract ActionResults GetInvalidOptionsResult();
    }
}