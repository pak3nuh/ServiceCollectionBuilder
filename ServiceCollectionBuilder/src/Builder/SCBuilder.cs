using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NCaro.DependencyInjection.Attributes;

namespace NCaro.DependencyInjection.Builder
{
    /// <summary>
    /// Entry point to create IServiceCollection instances from elements annotated with ServiceImplementationAttribute
    /// </summary>
    public class SCBuilder
    {
        private delegate IList<DiscoveredService> ScanFunc();
        private readonly IAttributeScanner _scanner;
        private readonly ICollection<DiscoveredService> _services = new List<DiscoveredService>();
        private readonly IList<Tuple<Type, object>> _liveServices = new List<Tuple<Type, object>>();

        private SCBuilder(IAttributeScanner scanner)
        {
            _scanner = scanner;
        }

        /// <summary>
        /// Builds the service collection
        /// </summary>
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
        
        /// <summary>
        /// Adds a live service to the builder. This service's lifecycle is not managed by builder
        /// so it will be added as a singleton.
        /// </summary>
        /// <param name="serviceInstance">The live service to add</param>
        /// <typeparam name="TS">Service interface type</typeparam>
        /// <typeparam name="TImpl">Service implementation type</typeparam>
        /// <returns>This builder</returns>
        public SCBuilder AddLiveService<TS,TImpl>(TImpl serviceInstance) where TS : class where TImpl : TS
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

        /// <summary>
        /// Adds all the classes annotted with <see cref="ServiceImplementationAttribute"/> in the assembly calling
        /// this method.
        /// </summary>
        public SCBuilder AddCurrentAssembly()
        {
            var curr = Assembly.GetCallingAssembly();
            return ScanAssembly(() => _scanner.scan(curr));
        }

        /// <summary>
        /// Adds all the classes annotted with <see cref="ServiceImplementationAttribute"/> in the assembly calling
        /// this method marked with a component id.
        /// </summary>
        /// <param name="component">The component id to filter</param>
        /// <param name="includeRootComponent">If true also adds the services without a component id</param>
        public SCBuilder AddCurrentAssembly(string component, bool includeRootComponent = false)
        {
            var curr = Assembly.GetCallingAssembly();
            return ScanAssembly(() => _scanner.scan(curr, component, includeRootComponent));
        }

        /// <summary>
        /// Adds all the classes annotted with <see cref="ServiceImplementationAttribute"/> in the assembly provided.
        /// </summary>
        /// <param name="assembly">The assembly to analyse</param>
        public SCBuilder AddAssembly(Assembly assembly)
        {
            return ScanAssembly(() => _scanner.scan(assembly));
        }

        /// <summary>
        /// Adds all the classes annotted with <see cref="ServiceImplementationAttribute"/> in the assembly provided
        /// marked with a component id.
        /// </summary>
        /// <param name="assembly">The assembly to analyse</param>
        /// <param name="component">The component id to filter</param>
        /// <param name="includeRootComponent">If true also adds the services without a component id</param>
        /// <returns></returns>
        public SCBuilder AddAssembly(Assembly assembly, string component, bool includeRootComponent = false)
        {
            return ScanAssembly(() => _scanner.scan(assembly, component, includeRootComponent));
        }

        private SCBuilder ScanAssembly(ScanFunc func)
        {
            foreach (var discoveredService in func())
            {
                _services.Add(discoveredService);
            }

            return this;
        }

        /// <summary>
        /// Makes the services registered by this builder configurable on each scope creation. See more info in
        /// <see cref="ContextSpiBuilder{TCtx}"/> 
        /// </summary>
        /// <typeparam name="TCtx">The configuration context type</typeparam>
        /// <returns>A builder for scoped context configuration</returns>
        public ContextSpiBuilder<TCtx> Context<TCtx>() where TCtx : class
        {
            return new ContextSpiBuilder<TCtx>(Build());
        }

        /// <summary>
        /// Creates a new SCBuilder.
        /// </summary>
        public static SCBuilder Create() {
            return new SCBuilder(new AttributeScanner());
        }

        /// <summary>
        /// Façade to create a <code>IServiceCollection</code> by scanning all the types annotated with <code>ServiceImplementationAttribute</code>
        /// in the assemby calling this method.
        /// </summary>
        /// <returns></returns>
        public static IServiceCollection FromCurrentAssembly()
        {
            var assembly = Assembly.GetCallingAssembly();
            return new SCBuilder(new AttributeScanner())
                .AddAssembly(assembly)
                .Build();
        }
    }
}