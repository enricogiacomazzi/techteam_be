using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Services;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ConfigureInfrastructure
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        var mock = new MockData();
        mock.Init();
        services.AddSingleton(mock);
        return services.Scan(s =>
            s.FromAssemblyOf<UserRepository>()
                .AddClasses(cls => cls.AssignableTo(typeof(IRepository<,>)))
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
        );
    }
}