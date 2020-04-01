using Autofac;

using KenticoInspector.Core;

namespace KenticoInspector.Reports
{
    public class ReportsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(ReportsModule).Assembly)
                .Where(type => type.IsClass)
                .Where(type => !type.IsAbstract)
                .Where(type => typeof(IReport).IsAssignableFrom(type))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}