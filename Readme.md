## ServiceCollectionBuilder

Utility that helps to build the dependency graph used by interconnected services by using custom attributes, instead of maintaining a manual service configuration.

It integrates with ``Microsoft.Extensions.DependencyInjection`` by providing a ``IServiceCollection`` object pre configured with the dependant services.

### Usage

Lest assume that our main component is the ``IHiFiveService`` interface.

```csharp
public interface IHiFiveService
{
    int HiFive();
}
```

and it implementation details need a ``ILogger`` instance.
```csharp
public class ServiceImpl : IHiFiveService
{

    private readonly ILogger _logger;

    public ServiceImpl(ILogger logger) {
        this._logger = logger;
    }

    public int HiFive()
    {
        _logger.debug("Sending HiFive");
        return 5;
    }
}
```
We can wire them up like
```csharp
[ServiceImplementation(typeof(IHiFiveService))]
public class ServiceImpl : IHiFiveService{
    ...
}

[ServiceImplementation(typeof(ILogger))]
public class ConsoleLogger : ILogger {
    ...
}
```
and get a fully constructed ``IHiFiveService``
```csharp
IHiFiveService service = ServiceCollectionBuilder
    .FromCurrentAssembly()
    .BuildServiceProvider()
    .GetService<IHiFiveService>();
```

### Changelog