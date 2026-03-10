using Firmaro.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Firmaro.Infrastructure.Data
{
    public class FirmaroDbContext : DbContext
    {
        public FirmaroDbContext(DbContextOptions<FirmaroDbContext> options) : base(options) 
        { 
        }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AutomationSettings> AutomationSettings { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ClientName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ClientPhone).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Status).HasConversion<string>();

                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            
            modelBuilder.Entity<AutomationSettings>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.HasOne<User>()
                      .WithOne()
                      .HasForeignKey<AutomationSettings>(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne<Appointment>()
                      .WithMany()
                      .HasForeignKey(e => e.AppointmentId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.BusinessName).HasMaxLength(100);
            });
        }
    }
}