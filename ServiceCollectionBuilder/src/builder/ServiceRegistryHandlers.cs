using System;
using Microsoft.Extensions.DependencyInjection;
using NCaro.ServiceCollectionBuilder.Attributes;

namespace NCaro.ServiceCollectionBuilder.Builder
{

    internal delegate void ServiceRegistryHandler(IServiceCollection serviceCollection, DiscoveredService discoveredService);
    
    internal static class ServiceRegistryHandlers
    {

        private static readonly ServiceRegistryHandler Singleton = (sc, ds) => { sc.AddSingleton(ds.Configuration.Interface, ds.Implementation); };
        private static readonly ServiceRegistryHandler Scoped = (sc, ds) => { sc.AddScoped(ds.Configuration.Interface, ds.Implementation); };
        private static readonly ServiceRegistryHandler Transient = (sc, ds) => { sc.AddTransient(ds.Configuration.Interface, ds.Implementation); };

        public static ServiceRegistryHandler GetServiceRegistryHandler(Scope scope)
        {
            switch (scope)
            {
                case Scope.Scoped: return Scoped;
                case Scope.Singleton: return Singleton;
                case Scope.Transient: return Transient; 
                default:
                    throw new ArgumentException($"Scope {scope} not supported");
            }
        }

    }
    
}