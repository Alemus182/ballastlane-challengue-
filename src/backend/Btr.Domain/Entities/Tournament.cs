namespace Btr.Domain.Entities;

public class Tournament
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}
