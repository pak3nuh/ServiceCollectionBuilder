using Microsoft.Extensions.DependencyInjection;
using pt.ncaro.util.dependencyinjection.builder;
using ServiceCollectionBuilderTests.attributes;
using Xunit;

namespace ServiceCollectionBuilderTests.builder
{
    public class BuilderTests
    {

        private ServiceProvider spi = ServiceCollectionBuilder.FromCurrentAssembly().BuildServiceProvider();
        
        [Fact]
        public void createServiceCollection()
        {
            var myService = spi.GetService<IHiFiveService>();
            Assert.NotNull(myService);
            Assert.Equal(5, myService.HiFive());
        }

        [Fact]
        public void multipleServiceImplementation()
        {
            var hiFiveService = spi.GetService<IHiFiveService>();
            Assert.NotNull(hiFiveService);
            var sayHelloService = spi.GetService<ISayHelloService>();
            Assert.NotNull(sayHelloService);
            //The implementation is the same, but the instance is not
            Assert.NotSame(hiFiveService, sayHelloService);
            Assert.Same(hiFiveService.GetType(), sayHelloService.GetType());
        }

        [Fact]
        public void scopedDependencies()
        {
            Assert.True(false);
        }

        [Fact]
        public void transientDependencies()
        {
            Assert.True(false);
        }
        
    }
}