namespace Btr.Application.Features.Registrations.Delete;

public sealed class DeleteRegistrationResult
{
    public bool Success { get; init; }

    public string? Error { get; init; }

    public static DeleteRegistrationResult Ok() =>
        new() { Success = true };

    public static DeleteRegistrationResult Fail(string error) =>
        new() { Success = false, Error = error };
}
