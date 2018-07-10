using System;

namespace pt.ncaro.util.dependencyinjection.attributes
{
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ServiceImplementationAttribute : Attribute
    {

        public const string RootComponent = "root";
        
        public Type Interface { get; }
        public Scope Scope { get; }
        public string Component { get; }

        public ServiceImplementationAttribute(Type @interface, Scope scope = Scope.Singleton, string component = RootComponent)
        {
            Interface = @interface;
            Scope = scope;
            Component = component;
        }
    }


    public enum Scope
    {
        Singleton, Scoped, Transient
    }
}