using Microsoft.EntityFrameworkCore;
using HalloDoc.Entities.Data.Entities;

namespace HalloDoc.Entities.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Example DbSet, replace with real entities
        public DbSet<Ping> Pings { get; set; }
    }
} 