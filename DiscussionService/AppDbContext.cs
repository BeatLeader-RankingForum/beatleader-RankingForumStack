using DiscussionService.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscussionService
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<MapDiscussion> MapDiscussions { get; set; }
        public DbSet<Discussion> Discussions { get; set; }

        internal static void ApplyMigrations(IHost app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var productContext = services.GetRequiredService<AppDbContext>();
                    productContext.Database.Migrate();
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

