using System;

using Autofac;
using Autofac.Extensions.DependencyInjection;

using KenticoInspector.Actions;
using KenticoInspector.Core;
using KenticoInspector.Instances;
using KenticoInspector.Modules;
using KenticoInspector.Reports;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Newtonsoft.Json.Serialization;

namespace KenticoInspector.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(options => { options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); });

            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "Client/dist"; });

            return ConfigureAutofac(services);
        }

        private static IServiceProvider ConfigureAutofac(IServiceCollection services)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);

            containerBuilder
                .RegisterModule(new CoreContainer())
                .RegisterModule(new InstancesContainer())
                .RegisterModule(new ModulesContainer())
                .RegisterModule(new ReportsContainer())
                .RegisterModule(new ActionsContainer());

            return new AutofacServiceProvider(containerBuilder.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env
            )
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            else app.UseHsts();

            app.UseStaticFiles()
                .UseSpaStaticFiles();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseSpa(
                spa =>
                {
                    spa.Options.SourcePath = "Client/dist";

                    if (env.IsDevelopment())
                        if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPNETCORE_USE_EXTERNAL_CLIENT"))
                            )
                            spa.UseProxyToSpaDevelopmentServer("http://localhost:8080");
                }
                );
        }
    }
}