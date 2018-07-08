using System;

namespace pt.ncaro.util.dependencyinjection.attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ServiceImplementationAttribute : Attribute
    {
        public Type Interface { get; }
        public Scope Scope { get; }

        public ServiceImplementationAttribute(Type @interface, Scope scope = Scope.Singleton)
        {
            Interface = @interface;
            Scope = scope;
        }
    }


    public enum Scope
    {
        Singleton, Scoped, Transient
    }
}