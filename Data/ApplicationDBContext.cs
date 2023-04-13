using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.Data
{
    public class ApplicationDBContext : DbContext
    {

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<Category>().HasData(
                new Category { Id=1, DisplayOrder = 1, Name="Action"},
                new Category { Id = 2, DisplayOrder = 2, Name = "SciFi" },
                new Category { Id = 3, DisplayOrder = 3, Name = "History" }

                );
        }

    }
}
