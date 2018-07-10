using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace pt.ncaro.util.dependencyinjection.scope
{
    public class ScopingTests
    {

        [Fact]
        public void Scopes()
        {
            var sc = new ServiceCollection();
            sc.AddScoped<IScoped, Scoped>();
            //option one use options pattern to create a new container
            //auto inject services from parent components
            //downside is the creation of a full container on each call
            sc.Configure<MyOptions>(opt => opt.Message = "cenas");
            var spi = sc.BuildServiceProvider();
            var scope1 = spi.CreateScope();
            var scope2 = spi.CreateScope();

            var s1 = spi.GetService<IScoped>();
            var s2 = spi.GetService<IScoped>();
            var s3 = scope1.ServiceProvider.GetService<IScoped>();
            var s4 = scope1.ServiceProvider.GetService<IScoped>();
            var s5 = scope2.ServiceProvider.GetService<IScoped>();
            
            Assert.Same(s1, s2);
            Assert.NotSame(s1, s3);
            Assert.Same(s3, s4);
            Assert.NotSame(s4, s5);
            Assert.NotSame(s1, s5);

            //option 2 try to use only scoped services on subcomponents
            //prevents the container recreaction with every contextualized call
            //does not allow singletons on subcomponents
        }
        
    }

    public interface IScoped {}
    
    public class Scoped : IScoped
    {
        
    }

    public class MyOptions
    {
        public string Message { get; set; }
    }
}