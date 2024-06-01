using DiscussionService.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscussionService
{
    public class DiscussionDbContext : DbContext
    {
        public DiscussionDbContext(DbContextOptions<DiscussionDbContext> options) : base(options)
        {
        }

        public DbSet<MapDiscussion> MapDiscussions { get; set; }
        public DbSet<DifficultyDiscussion> Discussions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MapDiscussion>()
                .HasIndex(d => d.MapsetId)
                .IsUnique();

            modelBuilder.Entity<DifficultyDiscussion>()
                .HasIndex(d => new { d.MapDiscussionId, d.Characteristic, d.Difficulty })
                .IsUnique();
        }

        internal static void ApplyMigrations(IHost app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var dbContext = services.GetRequiredService<DiscussionDbContext>();
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

    
}

