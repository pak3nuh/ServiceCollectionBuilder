using System;
using Microsoft.Extensions.DependencyInjection;

namespace NCaro.ServiceCollectionBuilder.Attributes
{
    
    /// <summary>
    /// Marks a class elegible for the implementation of a certain service
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ServiceImplementationAttribute : Attribute
    {

        public const string RootComponent = "root";
        
        /// <summary>
        /// The service interface exposed
        /// </summary>
        public Type Interface { get; }
        /// <summary>
        /// The scope of the service
        /// </summary>
        public Scope Scope { get; }
        /// <summary>
        /// The component associated.
        /// Components are sets of services coupled together by an identifier. The way attribute discovery
        /// works components are a usefull way to filter services from a larger group.
        /// </summary>
        public string Component { get; }

        public ServiceImplementationAttribute(Type @interface, Scope scope = Scope.Singleton, string component = RootComponent)
        {
            Interface = @interface;
            Scope = scope;
            Component = component;
        }
    }

    /// <summary>
    /// Service scope. They match a service provider scope.
    /// </summary>
    public enum Scope
    {
        Singleton, Scoped, Transient
    }
}