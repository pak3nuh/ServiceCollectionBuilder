using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using pt.ncaro.util.dependencyinjection.attributes;

namespace pt.ncaro.util.dependencyinjection.builder
{
    public class ServiceCollectionBuilder
    {
        private delegate IList<DiscoveredService> ScanFunc(Assembly assembly);

        private readonly IAttributeScanner _scanner;

        private readonly ICollection<DiscoveredService> _services = new List<DiscoveredService>();

        private ScanFunc _scanFunc;

        internal ServiceCollectionBuilder(IAttributeScanner scanner)
        {
            _scanner = scanner;
            _scanFunc = _scanner.scan;
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
            return ScanAssembly(Assembly.GetCallingAssembly());
        }

        public ServiceCollectionBuilder AddAssembly(Assembly assembly)
        {
            return ScanAssembly(assembly);
        }

        public ServiceCollectionBuilder Component(string component)
        {
            _scanFunc = assembly => _scanner.scan(assembly, component);
            return this;
        }

        private ServiceCollectionBuilder ScanAssembly(Assembly assembly)
        {
            foreach (var discoveredService in _scanFunc(assembly))
            {
                _services.Add(discoveredService);
            }

            return this;
        }

        public static IServiceCollection FromCurrentAssembly()
        {
            var assembly = Assembly.GetCallingAssembly();
            return new ServiceCollectionBuilder(new AttributeScanner())
                .ScanAssembly(assembly)
                .Build();
        }
    }
}