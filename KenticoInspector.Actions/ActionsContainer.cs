using Autofac;

using KenticoInspector.Core.Modules.Models;

namespace KenticoInspector.Actions
{
    public class ActionsContainer : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(ActionsContainer).Assembly)
                .Where(type => type.IsClass)
                .Where(type => !type.IsAbstract)
                .Where(type => typeof(IAction).IsAssignableFrom(type))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}