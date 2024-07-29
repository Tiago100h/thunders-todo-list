using ThundersTodoList.Domain.Interfaces.Services;
using ThundersTodoList.Infra;
using ThundersTodoList.Service;

namespace ThundersTodoList.Api.Configuration;

public static class DependencyInjectionConfig
{
    public static void AddDependencyInjectionConfig(this IServiceCollection services)
    {
        services.AddScoped<AppDbContext>();
        services.AddScoped<ITodoService, TodoService>();
    }
}