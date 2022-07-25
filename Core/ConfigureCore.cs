using Core.Interfaces.Services;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class ConfigureCore
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        return services.Scan(s =>
            s.FromAssemblyOf<UserService>()
                .AddClasses(cls => cls.Where(c => c.Name.EndsWith("Service")))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );
    }
}