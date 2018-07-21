using System;
using System.Collections.Generic;
using System.Reflection;
using NCaro.DependencyInjection.Mock;
using Xunit;

namespace NCaro.DependencyInjection.Attributes
{
    public class ScannerTests
    {

        private readonly IAttributeScanner _scanner = new AttributeScanner();
        
        /// <summary>
        /// Tests for all attributes in the current assembly. Every time a new attribute is used, this should
        /// be updated. Not the best tactic but not other way arround.
        /// </summary>
        [Fact]
        public void ScanCurrentAssemblyAllServices()
        {
            var attributes = _scanner.scan(Assembly.GetExecutingAssembly());
            
            Assert.Equal(8, attributes.Count);
            AssertImpl(attributes, typeof(ServiceImpl), typeof(IHiFiveService));
            AssertImpl(attributes, typeof(ServiceImpl), typeof(ISayHelloService));
            AssertImpl(attributes, typeof(ServiceImpl), typeof(IPingService));
            AssertImpl(attributes, typeof(ServiceImpl), typeof(IPongService));
        }

        private void AssertImpl(IEnumerable<DiscoveredService> services, Type implementation, Type serviceInterface)
        {
            Assert.Contains(services,it => implementation == it.Implementation && serviceInterface == it.Configuration.Interface);
        }

        [Fact]
        public void ScanSpecificComponent()
        {
            var attributes = _scanner.scan(Assembly.GetExecutingAssembly(), "ping");
            Assert.Equal(1, attributes.Count);
            AssertImpl(attributes, typeof(ServiceImpl), typeof(IPingService));
        }

        [Fact]
        public void ScanSpecificAndRootComponent()
        {
            var attributes = _scanner.scan(Assembly.GetExecutingAssembly(), "pong", true);
            Assert.Equal(3, attributes.Count);
            AssertImpl(attributes, typeof(ServiceImpl), typeof(IHiFiveService));
            AssertImpl(attributes, typeof(ServiceImpl), typeof(ISayHelloService));
            AssertImpl(attributes, typeof(ServiceImpl), typeof(IPongService));
        }
        
    }
}