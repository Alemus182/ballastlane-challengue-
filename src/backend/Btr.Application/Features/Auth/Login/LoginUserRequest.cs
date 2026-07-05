namespace Btr.Application.Features.Auth.Login;

public sealed record LoginUserRequest(string UsernameOrEmail, string Password);
