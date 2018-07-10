using System;

namespace pt.ncaro.util.dependencyinjection.scope
{

    //main
    public interface IMainService
    {
        ISlaveService GetSlaveService(SlaveOptions options);
    }
    
    public class MainService : IMainService
    {
        private readonly Func<SlaveOptions, ISlaveService> _producer;

        public MainService(Func<SlaveOptions, ISlaveService> producer)
        {
            _producer = producer;
        }

        public ISlaveService GetSlaveService(SlaveOptions options)
        {
            return _producer(options);
        }
    }

    //main
    public interface IAnotherService
    {
        int GiveFive();
    }

    public class SlaveOptions
    {
        public string Message { get; set; }
    }

    //depends on main
    public interface ISlaveService
    {
        string GetMessage();
    }
    
    public class Slave : ISlaveService
    {
        private readonly SlaveOptions _options;
        private readonly IAnotherService _service;

        public Slave(SlaveOptions options, IAnotherService service)
        {
            _options = options;
            _service = service;
        }

        public string GetMessage()
        {
            return $"Message is {_options.Message} and service {_service.GiveFive()}";
        }
    }
    
    //usar o options pattern para cada contexto
}