using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Application.Abstractions;
using TaskManagement.Infrastructure.Persistence;
using TaskManagement.Infrastructure.Repositories;

namespace TaskManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("TaskManagement")
            ?? "Data Source=task-management.db";

        services.AddDbContext<TaskManagementDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<ITaskRepository, TaskRepository>();

        return services;
    }
}