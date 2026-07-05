using Btr.Application.Abstractions.Persistence;
using Btr.Domain.Entities;

namespace Btr.Application.Features.Registrations.Get;

public sealed class GetRegistrationsService
{
    private readonly IRegistrationRepository _registrationRepository;

    public GetRegistrationsService(IRegistrationRepository registrationRepository)
    {
        _registrationRepository = registrationRepository;
    }

    public async Task<GetRegistrationsResult> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var registrations = await _registrationRepository.ListAsync(cancellationToken);

        var dtos = registrations
            .Select(ToDto)
            .ToList();

        return GetRegistrationsResult.Ok(dtos);
    }

    private static RegistrationDto ToDto(Registration registration)
    {
        return new RegistrationDto(
            registration.Id,
            registration.TournamentId,
            registration.PlayerName,
            registration.Nickname,
            registration.ContactInfo,
            registration.CreatedAtUtc);
    }
}
