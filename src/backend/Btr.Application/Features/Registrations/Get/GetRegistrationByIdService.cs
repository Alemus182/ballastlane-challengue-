using Btr.Application.Abstractions.Persistence;
using Btr.Domain.Entities;

namespace Btr.Application.Features.Registrations.Get;

public sealed class GetRegistrationByIdService
{
    private readonly IRegistrationRepository _registrationRepository;

    public GetRegistrationByIdService(IRegistrationRepository registrationRepository)
    {
        _registrationRepository = registrationRepository;
    }

    public async Task<GetRegistrationByIdResult> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var registration = await _registrationRepository.GetByIdAsync(id, cancellationToken);
        if (registration is null)
        {
            return GetRegistrationByIdResult.Fail("Registration not found.");
        }

        return GetRegistrationByIdResult.Ok(ToDto(registration));
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
