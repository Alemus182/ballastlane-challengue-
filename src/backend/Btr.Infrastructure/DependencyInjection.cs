using Btr.Application.Abstractions.Persistence;
using Btr.Application.Abstractions.Security;
using Btr.Infrastructure.Persistence;
using Btr.Infrastructure.Persistence.Repositories;
using Btr.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Btr.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=btr.db";

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        var jwtSection = configuration.GetSection("Jwt");
        services.Configure<JwtOptions>(options =>
        {
            options.Issuer = jwtSection["Issuer"] ?? string.Empty;
            options.Audience = jwtSection["Audience"] ?? string.Empty;
            options.Key = jwtSection["Key"] ?? string.Empty;
            options.ExpiresMinutes = int.TryParse(jwtSection["ExpiresMinutes"], out var minutes) ? minutes : 60;
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITournamentRepository, TournamentRepository>();
        services.AddScoped<IRegistrationRepository, RegistrationRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasherAdapter>();
        services.AddScoped<ITokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
