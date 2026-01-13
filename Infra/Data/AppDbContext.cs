using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> contextOptions) : DbContext(contextOptions)
    {
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Tag> Tags => Set<Tag>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
