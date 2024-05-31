using Contracts.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UserService.Models;

namespace UserService;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(u => u.Roles)
            .HasConversion(
                v => string.Join(",", v.Select(r => (int)r)), // Convert to comma-separated string of ints
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(r => (Role)int.Parse(r)).ToList() // Convert back to list of enums
            )
            .Metadata.SetValueComparer(new ValueComparer<ICollection<Role>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()));
    }

    internal static void ApplyMigrations(IHost app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var dbContext = services.GetRequiredService<AppDbContext>();
                dbContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while migrating the databases.");
                // Optionally throw, or handle the error as needed
            }
        }
    }
}