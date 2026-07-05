using Btr.Application.Abstractions.Persistence;
using Btr.Domain.Entities;

namespace Btr.Application.Features.Registrations.Create;

public sealed class CreateRegistrationService
{
    private readonly IRegistrationRepository _registrationRepository;
    private readonly ITournamentRepository _tournamentRepository;

    public CreateRegistrationService(
        IRegistrationRepository registrationRepository,
        ITournamentRepository tournamentRepository)
    {
        _registrationRepository = registrationRepository;
        _tournamentRepository = tournamentRepository;
    }

    public async Task<CreateRegistrationResult> ExecuteAsync(
        CreateRegistrationRequest request,
        Guid createdByUserId,
        CancellationToken cancellationToken = default)
    {
        if (request.TournamentId == Guid.Empty)
        {
            return CreateRegistrationResult.Fail("TournamentId is required.");
        }

        if (string.IsNullOrWhiteSpace(request.PlayerName))
        {
            return CreateRegistrationResult.Fail("PlayerName is required.");
        }

        if (string.IsNullOrWhiteSpace(request.ContactInfo))
        {
            return CreateRegistrationResult.Fail("ContactInfo is required.");
        }

        // Verify tournament exists
        var tournaments = await _tournamentRepository.GetAllAsync(cancellationToken);
        if (!tournaments.Any(t => t.Id == request.TournamentId))
        {
            return CreateRegistrationResult.Fail("Tournament not found.");
        }

        if (createdByUserId == Guid.Empty)
        {
            return CreateRegistrationResult.Fail("CreatedByUserId is required.");
        }

        var registration = new Registration
        {
            Id = Guid.NewGuid(),
            TournamentId = request.TournamentId,
            PlayerName = request.PlayerName.Trim(),
            Nickname = string.IsNullOrWhiteSpace(request.Nickname) ? null : request.Nickname.Trim(),
            ContactInfo = request.ContactInfo.Trim(),
            CreatedByUserId = createdByUserId,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _registrationRepository.AddAsync(registration, cancellationToken);
        return CreateRegistrationResult.Ok(registration.Id);
    }
}
