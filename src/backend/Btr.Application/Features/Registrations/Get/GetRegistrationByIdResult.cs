namespace Btr.Application.Features.Registrations.Get;

public sealed class GetRegistrationByIdResult
{
    public bool Success { get; init; }

    public RegistrationDto? Registration { get; init; }

    public string? Error { get; init; }

    public static GetRegistrationByIdResult Ok(RegistrationDto registration) =>
        new() { Success = true, Registration = registration };

    public static GetRegistrationByIdResult Fail(string error) =>
        new() { Success = false, Error = error };
}
