using Autofac;

using KenticoInspector.Core.TokenExpressions;

namespace KenticoInspector.Core
{
    public class CoreContainer : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            TokenExpressionResolver.RegisterTokenExpressions(typeof(CoreContainer).Assembly);
        }
    }
}