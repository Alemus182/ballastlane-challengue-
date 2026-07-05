namespace Btr.Application.Features.Auth.Login;

public sealed class LoginUserResult
{
    public bool Success { get; init; }

    public string? AccessToken { get; init; }

    public string? Error { get; init; }

    public static LoginUserResult Ok(string accessToken) =>
        new() { Success = true, AccessToken = accessToken };

    public static LoginUserResult Fail(string error) =>
        new() { Success = false, Error = error };
}
