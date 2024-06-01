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
    public DbSet<OrderedThreadItem> OrderedThreadItems { get; set; }
    public DbSet<Reply> Replies { get; set; }
    public DbSet<StatusUpdate> StatusUpdates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderedThreadItem>()
            .HasDiscriminator<string>("ItemType")
            .HasValue<Reply>("Reply")
            .HasValue<StatusUpdate>("StatusUpdate");
        
        modelBuilder.Entity<Comment>()
            .HasMany(c => c.Replies)
            .WithOne()
            .HasForeignKey(r => r.CommentId);
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