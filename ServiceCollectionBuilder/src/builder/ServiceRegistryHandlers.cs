using System;
using Microsoft.Extensions.DependencyInjection;
using pt.ncaro.util.dependencyinjection.attributes;

namespace pt.ncaro.util.dependencyinjection.builder
{

    internal delegate void ServiceRegistryHandler(IServiceCollection serviceCollection, DiscoveredService discoveredService);
    
    internal static class ServiceRegistryHandlers
    {

        private static ServiceRegistryHandler _singleton = (sc, ds) => { sc.AddSingleton(ds.Configuration.Interface, ds.Implementation); };
        private static ServiceRegistryHandler _scoped = (sc, ds) => { sc.AddScoped(ds.Configuration.Interface, ds.Implementation); };
        private static ServiceRegistryHandler _transient = (sc, ds) => { sc.AddTransient(ds.Configuration.Interface, ds.Implementation); };

        public static ServiceRegistryHandler GetServiceRegistryHandler(Scope scope)
        {
            switch (scope)
            {
                case Scope.Scoped: return _scoped;
                case Scope.Singleton: return _singleton;
                case Scope.Transient: return _transient; 
                default:
                    throw new ArgumentException($"Scope {scope} not supported");
            }
        }

    }
    
}