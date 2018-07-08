using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using pt.ncaro.util.dependencyinjection.attributes;

namespace pt.ncaro.util.dependencyinjection.builder
{
    public class ServiceCollectionBuilder
    {
        private readonly IAttributeScanner _scanner;
        private readonly ICollection<DiscoveredService> _services = new List<DiscoveredService>();

        internal ServiceCollectionBuilder(IAttributeScanner scanner)
        {
            _scanner = scanner;
        }

        public IServiceCollection Build()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            foreach (var s in _services)
            {
                addService(serviceCollection, s);
            }

            return serviceCollection;
        }

        private void addService(IServiceCollection serviceCollection, DiscoveredService service)
        {
            ServiceRegistryHandlers.GetServiceRegistryHandler(service.Configuration.Scope)(serviceCollection, service);
        }

        public ServiceCollectionBuilder AddCurrentAssembly()
        {
            return addAssembly(Assembly.GetCallingAssembly());
        }

        public ServiceCollectionBuilder AddAssembly(Assembly assembly)
        {
            return addAssembly(assembly);
        }

        private ServiceCollectionBuilder addAssembly(Assembly assembly)
        {
            foreach (var discoveredService in _scanner.scan(assembly))
            {
                _services.Add(discoveredService);
            }

            return this;
        }

        public static IServiceCollection FromCurrentAssembly()
        {
            var assembly = Assembly.GetCallingAssembly();
            return new ServiceCollectionBuilder(new AttributeScanner())
                .addAssembly(assembly)
                .Build();
        }
    }
}