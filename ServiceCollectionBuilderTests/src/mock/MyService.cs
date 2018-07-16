

using NCaro.ServiceCollectionBuilder.Attributes;

namespace NCaro.ServiceCollectionBuilder.Mock
{
    [ServiceImplementation(typeof(IHiFiveService))]
    [ServiceImplementation(typeof(ISayHelloService))]
    [ServiceImplementation(typeof(IPingService), Attributes.Scope.Transient, "ping")]
    [ServiceImplementation(typeof(IPongService), Attributes.Scope.Scoped, "pong")]
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