namespace Btr.Application.Features.Registrations.Get;

public sealed class GetRegistrationsResult
{
    public bool Success { get; init; }

    public IReadOnlyList<RegistrationDto> Registrations { get; init; } = [];

    public string? Error { get; init; }

    public static GetRegistrationsResult Ok(IReadOnlyList<RegistrationDto> items) =>
        new() { Success = true, Registrations = items };

    public static GetRegistrationsResult Fail(string error) =>
        new() { Success = false, Error = error };
}
