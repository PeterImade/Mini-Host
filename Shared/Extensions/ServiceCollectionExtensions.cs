using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Exceptions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddModules(this IServiceCollection services, IConfiguration configuration)
        {
            // Discover all assemblies that reference MiniHost.Shared
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Find all types that implement IModule
            var moduleTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToList();

            // Instantiate and register each module
            foreach (var type in moduleTypes)
            {
                var module = (IModule)Activator.CreateInstance(type)!;
                module.RegisterModule(services, configuration);
            }

            return services;
        }
    }
}
