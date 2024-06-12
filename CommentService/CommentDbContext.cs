using CommentService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CommentService;

public class CommentDbContext : DbContext
{
    public CommentDbContext(DbContextOptions<CommentDbContext> options) : base(options)
    {
    }

    public DbSet<Comment> Comments { get; set; }
    public DbSet<Reply> Replies { get; set; }
    public DbSet<StatusUpdate> StatusUpdates { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>()
            .HasMany(c => c.Replies)
            .WithOne()
            .HasForeignKey(r => r.CommentId);
        
        modelBuilder.Entity<Review>()
            .HasMany(c => c.Replies)
            .WithOne()
            .HasForeignKey(r => r.ReviewId);

        modelBuilder.Entity<Review>()
            .Property(r => r.CommentIds)
            .HasConversion(
                v => string.Join(';', v),
                v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList())
            .Metadata.SetValueComparer(new ValueComparer<ICollection<string>>(
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
                var dbContext = services.GetRequiredService<CommentDbContext>();
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