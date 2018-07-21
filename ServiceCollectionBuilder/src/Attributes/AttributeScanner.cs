using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NCaro.DependencyInjection.Attributes
{
    public interface IAttributeScanner
    {
        IList<DiscoveredService> scan(Assembly assembly);
        IList<DiscoveredService> scan(Assembly assembly, string component, bool includeRootComponent = false);
    }

    public class AttributeScanner : IAttributeScanner
    {
        public IList<DiscoveredService> scan(Assembly assembly)
        {
            return all(assembly).ToList();
        }

        public IList<DiscoveredService> scan(Assembly assembly, string component, bool includeRootComponent = false)
        {
            return all(assembly)
                .Where(it => includeRootComponent && it.Configuration.Component == ServiceImplementationAttribute.RootComponent || it.Configuration.Component == component)
                .ToList();
        }


        private IEnumerable<DiscoveredService> all(Assembly assembly)
        {
            return
                (from asType in assembly.DefinedTypes
                    select new {type = asType, attrs = asType.GetCustomAttributes<ServiceImplementationAttribute>()})
                .Where(it => it.attrs.Any())
                .SelectMany(it =>
                    from attr in it.attrs
                    select new DiscoveredService(it.type, attr)
                );
        }
    }

    public class DiscoveredService
    {
        public Type Implementation { get; }
        public ServiceImplementationAttribute Configuration { get; }

        public DiscoveredService(Type implementation, ServiceImplementationAttribute configuration)
        {
            Implementation = implementation;
            Configuration = configuration;
        }
    }
}