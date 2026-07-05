using Btr.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Btr.IntegrationTests;

/// <summary>
/// Custom factory that overrides the database connection to an isolated
/// in-memory SQLite instance, preventing file-level race conditions when
/// multiple <see cref="WebApplicationFactory{TEntryPoint}"/> instances run
/// in parallel. Automatically creates database schema on factory initialization.
/// </summary>

public sealed class BtrWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection;

    public BtrWebApplicationFactory()
    {
        // Create a persistent in-memory SQLite connection that survives
        // across multiple DbContext instances within the same test
        _connection = new SqliteConnection("DataSource=:memory:;Mode=Memory;Cache=Shared");
        _connection.Open();
    }

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.UseSetting(HostDefaults.EnvironmentKey, "Testing");

        builder.ConfigureServices(services =>
        {
            // Remove the real AppDbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor is not null)
                services.Remove(descriptor);

            // Replace with in-memory SQLite using the persistent connection
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(_connection));
            
            // Create database schema
            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                try
                {
                    dbContext.Database.EnsureCreated();
                }
                catch (SqliteException ex) when (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
                {
                    // A parallel host startup may attempt schema creation twice.
                    // Ignore this benign race in test bootstrap.
                }
            }
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _connection?.Dispose();
        }
        base.Dispose(disposing);
    }
}
