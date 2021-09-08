using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASP.NET_Humans.Models;
using Microsoft.EntityFrameworkCore;

namespace ASP.NET_Humans.Domain
{
    public sealed class AppDbContext : DbContext
    {
       // public DbSet<User> User { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<Cook> Cooks { get; set; }
        public DbSet<Сourier> Сouriers { get; set; }
        public DbSet<Company> Companies { get; set; }

        //public AppDbContext()
        //{
        //    Database.EnsureCreated();
        //}

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Worker>().HasOne(w => w.Company).WithMany(c => c.Workers)
                .HasForeignKey(w => w.CompanyId);

        }
    
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);
        //}
    }
}
