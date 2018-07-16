using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NCaro.ServiceCollectionBuilder.Attributes;
using NCaro.ServiceCollectionBuilder.Builder;
using NCaro.ServiceCollectionBuilder.Util;
using Xunit;

namespace NCaro.ServiceCollectionBuilder.Scope
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

        //Test is actually coorect but made ScopeContext internal
//        [Fact]
//        public void CreateScopedDependency()
//        {
//            var spi = new ServiceCollection()
//                .AddScoped<ScopedService>()
//                .AddScoped<IScopeCtx<Context>, ScopeContext<Context>>().BuildServiceProvider();
//
//            var scope1 = spi.CreateScope();
//            ((ScopeContext<Context>)scope1.ServiceProvider.GetRequiredService<IScopeCtx<Context>>()).SetContext(new Context() {Value = 3});
//            var value1 = scope1.ServiceProvider.GetRequiredService<ScopedService>().GetContextValue();
//            Assert.Equal(3, value1);
//
//            var scope2 = spi.CreateScope();
//            ((ScopeContext<Context>)scope2.ServiceProvider.GetRequiredService<IScopeCtx<Context>>()).SetContext(new Context() {Value = 50});
//            var value2 = scope2.ServiceProvider.GetRequiredService<ScopedService>().GetContextValue();
//            Assert.Equal(50, value2);
//            
//            //using old scopes
//            value1 = scope1.ServiceProvider.GetRequiredService<ScopedService>().GetContextValue();
//            Assert.Equal(3, value1);
//            value2 = scope2.ServiceProvider.GetRequiredService<ScopedService>().GetContextValue();
//            Assert.Equal(50, value2);
//        }

        [ServiceImplementation(typeof(ScopedService), scope:Attributes.Scope.Scoped, component:"scoped")]
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
            public int Value { get; }

            public Context(int value = 0)
            {
                Value = value;
            }
        }

        [Fact]
        public void TestScopeBuilder()
        {
            var sb = Builder.ServiceCollectionBuilder.Create()
                .AddAssembly(Assembly.GetExecutingAssembly(), "scoped")
                .Context<Context>();

            var spi1 = sb.CreateScope(new Context(123));
            var value1 = spi1.GetRequiredService<ScopedService>().GetContextValue();
            Assert.Equal(123, value1);
            
            var spi2 = sb.CreateScope(new Context(999));
            var value2 = spi2.GetRequiredService<ScopedService>().GetContextValue();
            Assert.Equal(999, value2);
            
            value1 = spi1.GetRequiredService<ScopedService>().GetContextValue();
            Assert.Equal(123, value1);
            value2 = spi2.GetRequiredService<ScopedService>().GetContextValue();
            Assert.Equal(999, value2);
        }
        
    }

    
}