using Btr.Application.Abstractions.Persistence;
using Btr.Application.Abstractions.Security;
using Btr.Domain.Entities;

namespace Btr.Application.Features.Auth.Register;

public sealed class RegisterUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterUserResult> ExecuteAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedUser = request.UsernameOrEmail.Trim();

        if (string.IsNullOrWhiteSpace(normalizedUser) || string.IsNullOrWhiteSpace(request.Password))
        {
            return RegisterUserResult.Fail("UsernameOrEmail and password are required.");
        }

        var existingUser = await _userRepository.GetByUsernameOrEmailAsync(normalizedUser, cancellationToken);
        if (existingUser is not null)
        {
            return RegisterUserResult.Fail("User already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            UsernameOrEmail = normalizedUser,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            CreatedAtUtc = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user, cancellationToken);
        return RegisterUserResult.Ok(user.Id);
    }
}
