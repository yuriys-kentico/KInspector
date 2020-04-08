using Autofac;

using KenticoInspector.Core.Repositories;
using KenticoInspector.Core.Services;

namespace KenticoInspector.Infrastructure
{
    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(InfrastructureModule).Assembly)
                .Where(type => type.IsClass)
                .Where(type => !type.IsAbstract)
                .Where(type => typeof(IService).IsAssignableFrom(type))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(typeof(InfrastructureModule).Assembly)
                .Where(type => type.IsClass)
                .Where(type => !type.IsAbstract)
                .Where(type => typeof(IRepository).IsAssignableFrom(type))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}