using Microsoft.Extensions.DependencyInjection;
using NCaro.ServiceCollectionBuilder.Mock;
using Xunit;

namespace NCaro.ServiceCollectionBuilder.Builder
{
    public class BuilderTests
    {

        private readonly ServiceProvider spi = ServiceCollectionBuilder.FromCurrentAssembly().BuildServiceProvider();
        
        [Fact]
        public void CreateServiceCollection()
        {
            var myService = spi.GetService<IHiFiveService>();
            Assert.NotNull(myService);
            Assert.Equal(5, myService.HiFive());
        }

        [Fact]
        public void MultipleServiceImplementation()
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
        public void ScopedDependencies()
        {
            Assert.Same(spi.GetService<IPongService>(),spi.GetService<IPongService>());
            var scope1 = spi.CreateScope();
            Assert.NotSame(scope1.ServiceProvider.GetService<IPongService>(),spi.GetService<IPongService>());
            Assert.Same(scope1.ServiceProvider.GetService<IPongService>(),scope1.ServiceProvider.GetService<IPongService>());
        }

        [Fact]
        public void TransientDependencies()
        {
            Assert.NotSame(spi.GetService<IPingService>(),spi.GetService<IPingService>());
        }
        
        [Fact]
        public void BuildComponent() {
            var comp = ServiceCollectionBuilder.Create().AddCurrentAssembly("ping").Build().BuildServiceProvider();
            Assert.NotNull(comp.GetService<IPingService>());
            Assert.Null(comp.GetService<IPongService>());
        }

    }
}