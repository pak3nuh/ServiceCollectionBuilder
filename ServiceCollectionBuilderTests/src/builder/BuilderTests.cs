using Microsoft.Extensions.DependencyInjection;
using pt.ncaro.util.dependencyinjection.mock;
using Xunit;

namespace pt.ncaro.util.dependencyinjection.builder
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
            Assert.Same(spi.GetService<IPongService>(),spi.GetService<IPongService>());
            var scope1 = spi.CreateScope();
            Assert.NotSame(scope1.ServiceProvider.GetService<IPongService>(),spi.GetService<IPongService>());
            Assert.Same(scope1.ServiceProvider.GetService<IPongService>(),scope1.ServiceProvider.GetService<IPongService>());
        }

        [Fact]
        public void transientDependencies()
        {
            Assert.NotSame(spi.GetService<IPingService>(),spi.GetService<IPingService>());
        }
        
    }
}