using Btr.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Btr.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Tournament> Tournaments => Set<Tournament>();

    public DbSet<Registration> Registrations => Set<Registration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.UsernameOrEmail).HasMaxLength(256).IsRequired();
            entity.Property(x => x.PasswordHash).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.HasIndex(x => x.UsernameOrEmail).IsUnique();
        });

        modelBuilder.Entity<Tournament>(entity =>
        {
            entity.ToTable("Tournaments");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(256).IsRequired();
            entity.Property(x => x.Location).HasMaxLength(512).IsRequired();
            entity.Property(x => x.StartDate).IsRequired();
            entity.Property(x => x.EndDate).IsRequired();
        });

        modelBuilder.Entity<Registration>(entity =>
        {
            entity.ToTable("Registrations");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TournamentId).IsRequired();
            entity.Property(x => x.PlayerName).HasMaxLength(256).IsRequired();
            entity.Property(x => x.Nickname).HasMaxLength(128);
            entity.Property(x => x.ContactInfo).HasMaxLength(512).IsRequired();
            entity.Property(x => x.CreatedByUserId).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();

            // Foreign keys
            entity.HasOne(r => r.Tournament)
                .WithMany()
                .HasForeignKey(r => r.TournamentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.CreatedByUser)
                .WithMany()
                .HasForeignKey(r => r.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            entity.HasIndex(x => x.TournamentId);
            entity.HasIndex(x => x.CreatedByUserId);
        });
    }
}
