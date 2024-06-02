using DiscussionService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DiscussionService
{
    public class DiscussionDbContext : DbContext
    {
        public DiscussionDbContext(DbContextOptions<DiscussionDbContext> options) : base(options)
        {
        }

        public DbSet<MapDiscussion> MapDiscussions { get; set; }
        public DbSet<DifficultyDiscussion> DifficultyDiscussions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MapDiscussion>()
                .HasIndex(d => d.MapsetId)
                .IsUnique();

            modelBuilder.Entity<DifficultyDiscussion>()
                .HasIndex(d => new { d.MapDiscussionId, d.Characteristic, d.Difficulty })
                .IsUnique();
            
            modelBuilder.Entity<MapDiscussion>()
                .HasMany(c => c.Discussions)
                .WithOne()
                .HasForeignKey(r => r.MapDiscussionId);
            
            modelBuilder.Entity<MapDiscussion>()
                .Property(r => r.DiscussionOwnerIds)
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

