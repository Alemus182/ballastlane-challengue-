namespace Btr.Application.Features.Auth.Register;

public sealed record RegisterUserRequest(string UsernameOrEmail, string Password);
