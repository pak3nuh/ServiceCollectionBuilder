using System.Reflection;
using pt.ncaro.util.dependencyinjection.attributes;
using Xunit;

namespace ServiceCollectionBuilderTests.attributes
{
    public class ScannerTests
    {

        [Fact]
        public void ScanCurrentAssembly()
        {
            var scanner = new AttributeScanner();
            var attributes = scanner.scan(Assembly.GetExecutingAssembly());
            
            Assert.Equal(2, attributes.Count);
            Assert.Contains(attributes,it => typeof(ServiceImpl) == it.Implementation && typeof(IHiFiveService) == it.Configuration.Interface);
            Assert.Contains(attributes,it => typeof(ServiceImpl) == it.Implementation && typeof(ISayHelloService) == it.Configuration.Interface);
        }
        
    }
}