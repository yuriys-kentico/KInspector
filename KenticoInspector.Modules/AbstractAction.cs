using KenticoInspector.Core.Modules.Models;
using KenticoInspector.Core.Modules.Models.Results;

using Newtonsoft.Json;

namespace KenticoInspector.Modules
{
    public abstract class AbstractAction<TTerms, TOptions> : AbstractModule<TTerms>, IAction
        where TTerms : new()
        where TOptions : new()
    {
        public ActionResults GetResults(string optionsJson)
        {
            TOptions options;

            try
            {
                options = JsonConvert.DeserializeObject<TOptions>(optionsJson);
            }
            catch
            {
                return GetInvalidOptionsResults();
            }

            return GetResults(options);
        }

        public abstract ActionResults GetResults(TOptions options);

        public abstract ActionResults GetInvalidOptionsResults();
    }
}