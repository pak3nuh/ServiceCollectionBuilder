using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using pt.ncaro.util.dependencyinjection.attributes;

namespace pt.ncaro.util.dependencyinjection.builder
{
    /// <summary>
    /// Entry point to create IServiceCollection instances from elements annotated with ServiceImplementationAttribute
    /// </summary>
    public class ServiceCollectionBuilder
    {
        private delegate IList<DiscoveredService> ScanFunc();
        private readonly IAttributeScanner _scanner;
        private readonly ICollection<DiscoveredService> _services = new List<DiscoveredService>();
        private readonly IList<Tuple<Type, object>> _liveServices = new List<Tuple<Type, object>>();

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
            foreach (var (type, instance) in _liveServices)
            {
                serviceCollection.AddSingleton(type, instance);
            }

            return serviceCollection;
        }
        
        public ServiceCollectionBuilder AddLiveService<TS,TImpl>(TImpl serviceInstance) where TS : class where TImpl : TS
        {
            //add scoped does not make sense here.
            //inherited services act as singletons because their lifecicle should be chained
            //if service B depends on scoped service A, then A is disposed B should be disposed
            _liveServices.Add(new Tuple<Type, object>(typeof(TS), serviceInstance));
            return this;
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

        public ServiceCollectionBuilder AddCurrentAssembly(string component, bool includeRootComponent = false)
        {
            var curr = Assembly.GetCallingAssembly();
            return ScanAssembly(() => _scanner.scan(curr, component, includeRootComponent));
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

        public ContextSpiBuilder<TCtx> Context<TCtx>() where TCtx : class
        {
            return new ContextSpiBuilder<TCtx>(Build());
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