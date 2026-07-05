using Btr.Domain.Entities;

namespace Btr.Application.Abstractions.Security;

public interface ITokenGenerator
{
    string GenerateToken(User user);
}
