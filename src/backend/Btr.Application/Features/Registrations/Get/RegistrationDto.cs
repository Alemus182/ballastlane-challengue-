namespace Btr.Application.Features.Registrations.Get;

public sealed record RegistrationDto(
    Guid Id,
    Guid TournamentId,
    string PlayerName,
    string? Nickname,
    string ContactInfo,
    DateTime CreatedAtUtc);
