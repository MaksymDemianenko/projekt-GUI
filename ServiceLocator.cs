using Microsoft.Extensions.DependencyInjection;

public static class ServiceLocator
{
    private static ServiceProvider _serviceProvider;

    public static void Initialize()
    {
        var services = new ServiceCollection();
        
        // Rejestracja usług
        services.AddSingleton<IProductService, ProductService>();
        
        _serviceProvider = services.BuildServiceProvider();
    }

    public static T GetService<T>() where T : class
    {
        return _serviceProvider.GetService<T>();
    }

    public static void Dispose()
    {
        _serviceProvider?.Dispose();
    }
}