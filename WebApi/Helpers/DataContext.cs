using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApi.Entities;

namespace WebApi.Helpers
{
    public class DataContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sql server database
            options.UseSqlServer(Configuration.GetConnectionString("WebApiDatabase"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.Balance)
                .HasDefaultValue(0);
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<Category> Categories { get; set; }
    }
}