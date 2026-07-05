using Btr.Application.Abstractions.Persistence;

namespace Btr.Application.Features.Registrations.Update;

public sealed class UpdateRegistrationService
{
    private readonly IRegistrationRepository _registrationRepository;
    private readonly ITournamentRepository _tournamentRepository;

    public UpdateRegistrationService(
        IRegistrationRepository registrationRepository,
        ITournamentRepository tournamentRepository)
    {
        _registrationRepository = registrationRepository;
        _tournamentRepository = tournamentRepository;
    }

    public async Task<UpdateRegistrationResult> ExecuteAsync(
        Guid registrationId,
        UpdateRegistrationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (registrationId == Guid.Empty)
        {
            return UpdateRegistrationResult.Fail("RegistrationId is required.");
        }

        if (request.TournamentId == Guid.Empty)
        {
            return UpdateRegistrationResult.Fail("TournamentId is required.");
        }

        if (string.IsNullOrWhiteSpace(request.PlayerName))
        {
            return UpdateRegistrationResult.Fail("PlayerName is required.");
        }

        if (string.IsNullOrWhiteSpace(request.ContactInfo))
        {
            return UpdateRegistrationResult.Fail("ContactInfo is required.");
        }

        var existing = await _registrationRepository.GetByIdAsync(registrationId, cancellationToken);
        if (existing is null)
        {
            return UpdateRegistrationResult.Fail("Registration not found.");
        }

        var tournaments = await _tournamentRepository.GetAllAsync(cancellationToken);
        if (!tournaments.Any(t => t.Id == request.TournamentId))
        {
            return UpdateRegistrationResult.Fail("Tournament not found.");
        }

        existing.TournamentId = request.TournamentId;
        existing.PlayerName = request.PlayerName.Trim();
        existing.Nickname = string.IsNullOrWhiteSpace(request.Nickname) ? null : request.Nickname.Trim();
        existing.ContactInfo = request.ContactInfo.Trim();

        await _registrationRepository.UpdateAsync(existing, cancellationToken);
        return UpdateRegistrationResult.Ok();
    }
}
