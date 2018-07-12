using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using pt.ncaro.util.dependencyinjection.attributes;
using pt.ncaro.util.dependencyinjection.builder;
using pt.ncaro.util.dependencyinjection.util;
using Xunit;

namespace pt.ncaro.util.dependencyinjection.scope
{

    interface IS1
    {
        int dep1();
    }

    interface IS2
    {
        int dep2();
    }

    interface ID1
    {
        int dep3();
    }

    [ServiceImplementation(typeof(IS1), component:"main")]
    class S1 : IS1
    {
        private readonly Opt _options;

        public S1(IOptions<Opt> options)
        {
            _options = options.Value;
        }

        public int dep1()
        {
            return 1;
        }
    }

    [ServiceImplementation(typeof(IS2), component:"main")]
    class S2 : IS2
    {
        public int dep2()
        {
            return 2;
        }
    }

    [ServiceImplementation(typeof(ID1), component:"slave")]
    class D1:ID1
    {
        public int dep3()
        {
            return 3;
        }
    }

    class Opt
    {
        public string Message { get; set; }
    }

    public class TestClass
    {

        [Fact]
        public void CreateDependency()
        {
            var main = ServiceCollectionBuilder.Create()
                .AddCurrentAssembly("main").Build();
            
            var slave = ServiceCollectionBuilder.Create()
                .AddCurrentAssembly("slave").Build();

            
        }

        [Fact]
        public void ConfigureIsNotCalledOnScope()
        {
            var counter = 0;
            var spi = new ServiceCollection()
                .AddScoped<IS1, S1>()
                .Configure<Opt>(o =>
                {
                    counter++;
                    o.Message = System.DateTime.UtcNow.ToLongTimeString();
                })
                .BuildServiceProvider();

            Assert.Equal(0, counter);
            spi.GetRequiredService<IS1>();
            Assert.Equal(1, counter);
            spi.GetRequiredService<IS1>();
            Assert.Equal(1, counter);
            spi.CreateScope().ServiceProvider.GetRequiredService<IS1>();
            Assert.Equal(1, counter);

        }

        [Fact]
        public void CreateScopedDependency()
        {
            var spi = new ServiceCollection()
                .AddScoped<ScopedService>()
                .AddScoped<IScopeCtx<Context>, ScopeContext<Context>>().BuildServiceProvider();

            var scope1 = spi.CreateScope();
            ((ScopeContext<Context>)scope1.ServiceProvider.GetRequiredService<IScopeCtx<Context>>()).SetContext(new Context() {Value = 3});
            var value1 = scope1.ServiceProvider.GetRequiredService<ScopedService>().GetContextValue();
            Assert.Equal(3, value1);

            var scope2 = spi.CreateScope();
            ((ScopeContext<Context>)scope2.ServiceProvider.GetRequiredService<IScopeCtx<Context>>()).SetContext(new Context() {Value = 50});
            var value2 = scope2.ServiceProvider.GetRequiredService<ScopedService>().GetContextValue();
            Assert.Equal(50, value2);
            
            //using old scopes
            value1 = scope1.ServiceProvider.GetRequiredService<ScopedService>().GetContextValue();
            Assert.Equal(3, value1);
            value2 = scope2.ServiceProvider.GetRequiredService<ScopedService>().GetContextValue();
            Assert.Equal(50, value2);
        }

        [ServiceImplementation(typeof(ScopedService), component:"scoped")]
        class ScopedService
        {
            private readonly Context _context;

            public ScopedService(IScopeCtx<Context> context)
            {
                _context = context.Context;
            }

            public int GetContextValue()
            {
                return _context.Value;
            }
        }

        class Context
        {
            public int Value { get; set; }

            public Context(int value = 0)
            {
                Value = value;
            }
        }

        [Fact]
        public void TestScopeBuilder()
        {
            var sb = new ScopeBuilder()
                .Assembly(Assembly.GetExecutingAssembly())
                .Component("scoped")
                .Context<Context>();

            var spi1 = sb.CreateScope(new Context(123));
            var value1 = spi1.GetRequiredService<ScopedService>().GetContextValue();
            Assert.Equal(123, value1);
            
            var spi2 = sb.CreateScope(new Context(999));
            var value2 = spi1.GetRequiredService<ScopedService>().GetContextValue();
            Assert.Equal(999, value2);
            
            var value = spi1.GetRequiredService<ScopedService>().GetContextValue();
            Assert.Equal(123, value1);
            value2 = spi1.GetRequiredService<ScopedService>().GetContextValue();
            Assert.Equal(999, value2);
        }
        
    }

    public class ScopeBuilder
    {
        private string _component;
        private IList<Assembly> _assemblies = new List<Assembly>();
        private IList<Tuple<Type, object>> _services = new List<Tuple<Type, object>>();

        public ScopeBuilder Component(string component)
        {
            _component = component;
            return this;
        }

        public ScopeBuilder Assembly(Assembly assembly)
        {
            _assemblies.Add(assembly);
            return this;
        }

        public ScopeBuilder InheritService<S,I>(I serviceInstance) where S : class where I : S
        {
            _services.Add(new Tuple<Type, object>(typeof(S), serviceInstance));
            return this;
        }

        public ScopedSpiBuilder<C> Context<C>() where C : class
        {
            Check.State(_assemblies.Any(), "Assemblies not provided");
            
            var scb = ServiceCollectionBuilder.Create();
            foreach (var assembly in _assemblies)
            {
                scb.AddAssembly(assembly, _component);
            }

            var sc = scb.Build();
            foreach (var (type, instance) in _services)
            {
                //add scoped does not make sense here.
                //inherited services act as singletons because their lifecicle should be chained
                //if service B depends on scoped service A, then A is disposed B should be disposed
                sc.AddSingleton(type, instance);
            }

            sc.AddScoped<IScopeCtx<C>, ScopeContext<C>>();
            return new ScopedSpiBuilder<C>(sc.BuildServiceProvider());
        }
    }

    public class ScopedSpiBuilder<C> where C : class
    {
        private readonly IServiceProvider _spi;

        public ScopedSpiBuilder(IServiceProvider spi)
        {
            _spi = spi;
        }

        public IServiceProvider CreateScope(C context)
        {
            var newSpi = _spi.CreateScope().ServiceProvider;
            var contextService = (ScopeContext<C>)newSpi.GetRequiredService<IScopeCtx<C>>();
            contextService.SetContext(context);
            return newSpi;
        }
    }

    interface IScopeCtx<C>
    {
        C Context { get; }
    }
    
    //Cannot segregate set in different interface because 2 instances would be created
    internal class ScopeContext<C> : IScopeCtx<C>
    {
        public C Context { get; private set; }

        public void SetContext(C context)
        {
            Context = context;
        }
    }
}