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

        public DbSet<Ping> Pings { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Physician> Physicians { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Menus> Menus { get; set; }
        public DbSet<RoleMenus> RoleMenus { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<RequestClient> RequestClients { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<AgreementToken> AgreementTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();
            });

            modelBuilder.Entity<Admin>()
                .HasOne(a => a.Role)
                .WithMany(r => r.Admins)
                .HasForeignKey(a => a.RoleId);

            modelBuilder.Entity<Physician>()
                .HasOne(p => p.Role)
                .WithMany(r => r.Physicians)
                .HasForeignKey(p => p.RoleId);

            modelBuilder.Entity<Admin>()
                .HasOne(a => a.User)
                .WithMany(u => u.Admins)
                .HasForeignKey(a => a.UserId);

            modelBuilder.Entity<Physician>()
                .HasOne(p => p.User)
                .WithMany(u => u.Physicians)
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<Patient>()
                .HasOne(p => p.User)
                .WithMany(u => u.Patients)
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<RoleMenus>()
                .HasOne(rm => rm.Role)
                .WithMany(r => r.RoleMenus)
                .HasForeignKey(rm => rm.RoleId);

            modelBuilder.Entity<RoleMenus>()
                .HasOne(rm => rm.Menu)
                .WithMany(m => m.RoleMenus)
                .HasForeignKey(rm => rm.MenuId);

            // Request/RequestClient/Document relationships
            modelBuilder.Entity<Request>()
                .HasMany(r => r.RequestClients)
                .WithOne(r => r.Request)
                .HasForeignKey(rc => rc.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Request>()
                .HasMany(r => r.Documents)
                .WithOne(d => d.Request)
                .HasForeignKey(d => d.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Request>()
                .HasMany(r => r.AgreementTokens)
                .WithOne(a => a.Request)
                .HasForeignKey(a => a.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Request>()
                .HasOne(r => r.Patient)
                .WithMany(r => r.Requests)
                .HasForeignKey(r => r.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Request>()
                .HasOne(r => r.Physician)
                .WithMany(p => p.Requests)
                .HasForeignKey(r => r.PhysicianId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 