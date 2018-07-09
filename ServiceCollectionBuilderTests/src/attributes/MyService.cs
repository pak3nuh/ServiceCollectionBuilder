﻿using pt.ncaro.util.dependencyinjection.attributes;

namespace ServiceCollectionBuilderTests.attributes
{
    [ServiceImplementation(typeof(IHiFiveService))]
    [ServiceImplementation(typeof(ISayHelloService))]
    public class ServiceImpl : IHiFiveService, ISayHelloService
    {
        public int HiFive()
        {
            return 5;
        }

        public string Hello()
        {
            return "Hello";
        }
    }

    public interface IHiFiveService
    {
        int HiFive();
    }

    public interface ISayHelloService
    {
        string Hello();
    }
}