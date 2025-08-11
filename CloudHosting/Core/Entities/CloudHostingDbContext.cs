using CloudHosting.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CloudHosting.Infrastructure.Data
{
    public class CloudHostingDbContext(DbContextOptions<CloudHostingDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<UserPlan> UserPlans { get; set; }
        public DbSet<UserFile> UserFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User Plans relationship
            modelBuilder.Entity<UserPlan>()
                .HasOne(up => up.User)
                .WithMany(u => u.Plans)
                .HasForeignKey(up => up.UserId);

            modelBuilder.Entity<UserPlan>()
                .HasOne(up => up.Plan)
                .WithMany()
                .HasForeignKey(up => up.PlanId);

            // Configure User Files relationship
            modelBuilder.Entity<UserFile>()
                .HasOne(uf => uf.User)
                .WithMany(u => u.Files)
                .HasForeignKey(uf => uf.UserId);

            // Add indexes
            modelBuilder.Entity<UserFile>()
                .HasIndex(uf => uf.StorageFileName);

            modelBuilder.Entity<UserPlan>()
                .HasIndex(up => up.TransactionId);

            // Seed initial plans
            modelBuilder.Entity<Plan>().HasData(
                new Plan 
                { 
                    Id = 1,
                    Name = "Basic",
                    Description = "Basic plan with 1 CPU core and 1GB RAM",
                    Price = 9.99M,
                    MaxCpuCores = 1,
                    MaxMemoryMB = 1024,
                    MaxStorageGB = 10,
                    IsActive = true
                }
            );
        }
    }
}