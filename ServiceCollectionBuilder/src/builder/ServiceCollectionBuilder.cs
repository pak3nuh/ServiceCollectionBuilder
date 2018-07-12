using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using pt.ncaro.util.dependencyinjection.attributes;

namespace pt.ncaro.util.dependencyinjection.builder
{
    public class ServiceCollectionBuilder
    {
        private delegate IList<DiscoveredService> ScanFunc();

        private readonly IAttributeScanner _scanner;

        private readonly ICollection<DiscoveredService> _services = new List<DiscoveredService>();

        private ServiceCollectionBuilder(IAttributeScanner scanner)
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
            var curr = Assembly.GetCallingAssembly();
            return ScanAssembly(() => _scanner.scan(curr));
        }

        public ServiceCollectionBuilder AddCurrentAssembly(string component)
        {
            var curr = Assembly.GetCallingAssembly();
            return ScanAssembly(() => _scanner.scan(curr, component));
        }

        public ServiceCollectionBuilder AddAssembly(Assembly assembly)
        {
            return ScanAssembly(() => _scanner.scan(assembly));
        }

        public ServiceCollectionBuilder AddAssembly(Assembly assembly, string component, bool includeRootComponent = false)
        {
            return ScanAssembly(() => _scanner.scan(assembly, component, includeRootComponent));
        }

        private ServiceCollectionBuilder ScanAssembly(ScanFunc func)
        {
            foreach (var discoveredService in func())
            {
                _services.Add(discoveredService);
            }

            return this;
        }

        public static ServiceCollectionBuilder Create() {
            return new ServiceCollectionBuilder(new AttributeScanner());
        }

        public static IServiceCollection FromCurrentAssembly()
        {
            var assembly = Assembly.GetCallingAssembly();
            return new ServiceCollectionBuilder(new AttributeScanner())
                .AddAssembly(assembly)
                .Build();
        }
    }
}