using Btr.Application.Abstractions.Persistence;
using Btr.Application.Abstractions.Security;

namespace Btr.Application.Features.Auth.Login;

public sealed class LoginUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;

    public LoginUserService(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<LoginUserResult> ExecuteAsync(LoginUserRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedUser = request.UsernameOrEmail.Trim();

        if (string.IsNullOrWhiteSpace(normalizedUser) || string.IsNullOrWhiteSpace(request.Password))
        {
            return LoginUserResult.Fail("UsernameOrEmail and password are required.");
        }

        var user = await _userRepository.GetByUsernameOrEmailAsync(normalizedUser, cancellationToken);
        if (user is null)
        {
            return LoginUserResult.Fail("Invalid credentials.");
        }

        var isValidPassword = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
        if (!isValidPassword)
        {
            return LoginUserResult.Fail("Invalid credentials.");
        }

        var token = _tokenGenerator.GenerateToken(user);
        return LoginUserResult.Ok(token);
    }
}
