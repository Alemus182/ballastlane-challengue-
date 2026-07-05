namespace Btr.Application.Features.Registrations.Update;

public sealed class UpdateRegistrationResult
{
    public bool Success { get; init; }

    public string? Error { get; init; }

    public static UpdateRegistrationResult Ok() =>
        new() { Success = true };

    public static UpdateRegistrationResult Fail(string error) =>
        new() { Success = false, Error = error };
}
