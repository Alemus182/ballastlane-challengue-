namespace Btr.Domain.Entities;

public class Registration
{
    public Guid Id { get; set; }

    public Guid TournamentId { get; set; }

    public string PlayerName { get; set; } = string.Empty;

    public string? Nickname { get; set; }

    public string ContactInfo { get; set; } = string.Empty;

    public Guid CreatedByUserId { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    // Relationships
    public virtual Tournament? Tournament { get; set; }

    public virtual User? CreatedByUser { get; set; }
}
