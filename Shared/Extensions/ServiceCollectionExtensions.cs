using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Exceptions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddModules(this IServiceCollection services, IConfiguration configuration)
        {
            // Force-load all assemblies that start with "Modules."
            var baseDir = AppContext.BaseDirectory;
            var moduleDlls = Directory.GetFiles(baseDir, "Modules.*.dll", SearchOption.TopDirectoryOnly);

            foreach (var dll in moduleDlls)
            {
                try
                {
                    var assemblyName = AssemblyName.GetAssemblyName(dll);
                    if (AppDomain.CurrentDomain.GetAssemblies().All(a => a.GetName().Name != assemblyName.Name))
                    {
                        Assembly.Load(assemblyName);
                    }
                }
                catch (Exception ex)
                {
                }
            }

            // Discover all assemblies
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Find all types implementing IModule
            var moduleTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .ToList();

            // Instantiate and register
            foreach (var type in moduleTypes)
            {
                var module = (IModule)Activator.CreateInstance(type)!;
                module.RegisterModule(services, configuration);
            }

            return services;
        }
    }

}
