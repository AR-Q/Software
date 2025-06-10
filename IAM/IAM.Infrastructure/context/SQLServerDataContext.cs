using IAM.Application.Common.Hash;
using IAM.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace IAM.Infrastructure.context
{
    public class SQLServerDataContext  : DbContext
    {
        private readonly IHasher _haser;
        public SQLServerDataContext(DbContextOptions<SQLServerDataContext> options, IHasher hasher) : base(options)
        {
            _haser = hasher;
        }


        public DbSet<Admin> Admins { get; set; }
        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Admin>().HasKey(a => a.AdminId);
            modelBuilder.Entity<User>().HasKey(u => u.UserId);

            modelBuilder.Entity<Admin>().HasData(new Admin() { AdminId = 1, Email = "admin@gmail.com", Password = _haser.Hash("1234") });
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server = .;Database=IAMSkyDock;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true");
        }
    }
}
