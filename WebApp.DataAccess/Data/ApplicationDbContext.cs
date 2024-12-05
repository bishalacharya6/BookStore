using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Web.Models;

namespace Web.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
            
        }



        public DbSet<Catagory> Catagories { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ApplicationUsers> ApplicationUsers { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<Company> Company { get; set; }
            
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Company>().HasData(
                new Company
                {
                    Id = 1,
                    Name = "TechCorp",
                    StreetAddress = "123 Innovation Drive",
                    City = "Techville",
                    State = "CA",
                    PostalCode = "90210",
                    PhoneNumber = 1234567890
                },
                new Company
                {
                    Id = 2,
                    Name = "Green Solutions",
                    StreetAddress = "456 Eco Street",
                    City = "Greenfield",
                    State = "TX",
                    PostalCode = "73301",
                    PhoneNumber = 9876543210
                },
                new Company
                {
                    Id = 3,
                    Name = "Blue Ocean Industries",
                    StreetAddress = "789 Ocean Avenue",
                    City = "Seaside",
                    State = "FL",
                    PostalCode = "33101",
                    PhoneNumber = 5551234567
                }
            );

        }

    }
}
