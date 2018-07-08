using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace pt.ncaro.util.dependencyinjection.attributes
{
    public interface IAttributeScanner
    {
        IList<DiscoveredService> scan(Assembly assembly);
    }

    public class AttributeScanner : IAttributeScanner
    {
        public IList<DiscoveredService> scan(Assembly assembly)
        {
            return
                (from t in assembly.DefinedTypes
                    select new {type = t, attrs = t.GetCustomAttributes<ServiceImplementationAttribute>()})
                .Where(it => it.attrs.Any())
                .SelectMany(it =>
                    from attr in it.attrs
                    select new DiscoveredService(it.type, attr)
                )
                .ToList();
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