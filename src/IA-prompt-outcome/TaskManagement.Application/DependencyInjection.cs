using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Application.Features.Tasks;

namespace TaskManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITaskService, TaskService>();
        return services;
    }
}