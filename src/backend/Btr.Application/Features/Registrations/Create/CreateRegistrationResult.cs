namespace Btr.Application.Features.Registrations.Create;

public sealed class CreateRegistrationResult
{
    public bool Success { get; init; }

    public Guid? RegistrationId { get; init; }

    public string? Error { get; init; }

    public static CreateRegistrationResult Ok(Guid id) =>
        new() { Success = true, RegistrationId = id };

    public static CreateRegistrationResult Fail(string error) =>
        new() { Success = false, Error = error };
}
