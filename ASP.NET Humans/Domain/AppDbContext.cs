using ASP.NET_Humans.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASP.NET_Humans.Domain
{
    public sealed class AppDbContext : IdentityDbContext<User>
    {
        public DbSet<Worker> Workers { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
