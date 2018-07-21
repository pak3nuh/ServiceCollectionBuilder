## ServiceCollectionBuilder

Utility that helps to build the dependency graph used by interconnected services by using custom attributes, instead of maintaining a manual service configuration.

It integrates with ``Microsoft.Extensions.DependencyInjection`` by providing a ``IServiceCollection`` object pre configured with the dependant services.

Available on [nuget](https://www.nuget.org/packages/ServiceCollectionBuilder)

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
IHiFiveService service = SCBuilder
    .FromCurrentAssembly()
    .BuildServiceProvider()
    .GetService<IHiFiveService>();
```

### Components

Is possible to assign a component ID to each ``ServiceImplementation`` in order to group services that should work together. This way the same assembly can be used to supply instances of ``IServiceCollection`` with diffent services.

### Scoped Contexts

One of the limitations I found with the current ``IServiceCollection`` is that the [options pattern](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-2.1) registers a Singleton service for context data.

Inspired by the same pattern ``ContextSpiBuilder`` can create Scoped ``IServiceProvider`` instances passing a different context instance every time.

Services can access the scope context by requiring a depedency of type ``IScopeCtx<T>`` where ``T`` is the context type.

```csharp
public class Context
{
    public int Value { get; }

    public Context(int value = 0) { Value = value; }
}

[ServiceImplementation(typeof(ScopedService), scope:Attributes.Scope.Scoped, component:"scoped")]
public class ScopedService
{
    private readonly Context _context;

    public ScopedService(IScopeCtx<Context> context) { _context = context.Context; }

    public int GetContextValue() { return _context.Value; }
}

public void TestScopeBuilder()
{
    ContextSpiBuilder<Context> sb = SCBuilder.Create()
        .AddAssembly(Assembly.GetExecutingAssembly(), "scoped")
        .Context<Context>();

    var value1 = sb.CreateScope(new Context(123)).GetRequiredService<ScopedService>().GetContextValue();
    Assert.Equal(123, value1);
    
    var value2 = sb.CreateScope(new Context(999)).GetRequiredService<ScopedService>().GetContextValue();
    Assert.Equal(999, value2);
}
```

### Changelog

- v0.1.2-beta Applied namespace conventions.
Added more documentation.

- v0.1.0-beta Initial beta release
