namespace Btr.Application.Features.Auth.Register;

public sealed class RegisterUserResult
{
    public bool Success { get; init; }

    public Guid? UserId { get; init; }

    public string? Error { get; init; }

    public static RegisterUserResult Ok(Guid userId) =>
        new() { Success = true, UserId = userId };

    public static RegisterUserResult Fail(string error) =>
        new() { Success = false, Error = error };
}
