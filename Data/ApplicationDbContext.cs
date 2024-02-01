using Microsoft.EntityFrameworkCore;
using web_based.Models;

namespace web_based.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data for the Category table
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Friend" },
                new Category { Id = 2, Name = "Work" },
                new Category { Id = 3, Name = "Family" }
            );
        }
    }
}
