using Btr.Application.Features.Auth.Login;
using Btr.Application.Features.Auth.Register;
using Btr.Application.Features.Registrations.Create;
using Btr.Application.Features.Registrations.Delete;
using Btr.Application.Features.Registrations.Get;
using Btr.Application.Features.Registrations.Update;
using Btr.Application.Features.Tournaments.Create;
using Btr.Application.Features.Tournaments.GetList;
using Microsoft.Extensions.DependencyInjection;

namespace Btr.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<RegisterUserService>();
        services.AddScoped<LoginUserService>();
        services.AddScoped<CreateTournamentService>();
        services.AddScoped<GetTournamentsService>();
        services.AddScoped<CreateRegistrationService>();
        services.AddScoped<GetRegistrationsService>();
        services.AddScoped<GetRegistrationByIdService>();
        services.AddScoped<UpdateRegistrationService>();
        services.AddScoped<DeleteRegistrationService>();

        return services;
    }
}
