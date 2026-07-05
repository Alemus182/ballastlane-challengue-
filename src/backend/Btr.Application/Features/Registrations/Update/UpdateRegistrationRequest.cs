namespace Btr.Application.Features.Registrations.Update;

public sealed record UpdateRegistrationRequest(
    Guid TournamentId,
    string PlayerName,
    string? Nickname,
    string ContactInfo);
