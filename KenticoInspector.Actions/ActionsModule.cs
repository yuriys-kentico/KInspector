using Autofac;

using KenticoInspector.Core.Modules;

namespace KenticoInspector.Actions
{
    public class ActionsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(ActionsModule).Assembly)
                .Where(type => type.IsClass)
                .Where(type => !type.IsAbstract)
                .Where(type => typeof(IAction).IsAssignableFrom(type))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}