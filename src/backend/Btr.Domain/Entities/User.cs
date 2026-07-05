namespace Btr.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public string UsernameOrEmail { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }
}
