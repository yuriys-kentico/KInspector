using Autofac;

using KenticoInspector.Core.Tokens;

namespace KenticoInspector.Core
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            TokenExpressionResolver.RegisterTokenExpressions(typeof(CoreModule).Assembly);
        }
    }
}