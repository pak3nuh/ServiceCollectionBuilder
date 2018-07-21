

using NCaro.DependencyInjection.Attributes;

namespace NCaro.DependencyInjection.Mock
{
    [ServiceImplementation(typeof(IHiFiveService))]
    [ServiceImplementation(typeof(ISayHelloService))]
    [ServiceImplementation(typeof(IPingService), DependencyInjection.Attributes.Scope.Transient, "ping")]
    [ServiceImplementation(typeof(IPongService), DependencyInjection.Attributes.Scope.Scoped, "pong")]
    public class ServiceImpl : IHiFiveService, ISayHelloService, IPingService, IPongService
    {
        public int HiFive()
        {
            return 5;
        }

        public string Hello()
        {
            return "Hello";
        }

        public string Ping()
        {
            return "ping";
        }

        public string Pong()
        {
            return "pong";
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

    public interface IPingService
    {
        string Ping();
    }

    public interface IPongService
    {
        string Pong();
    }
}