using Microsoft.EntityFrameworkCore;
using RoleBasedBasicAuthentication.Models;
using RolesBasedAuthentication.Models;

namespace RoleBasedBasicAuthentication.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure a composite primary key for UserRole
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            // Configure many-to-many relationships between User and Role via UserRole.
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // Seed initial users
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Password = "password123" },
                new User { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com", Password = "password123" },
                new User { Id = 3, FirstName = "Hina", LastName = "Sharma", Email = "hina.sharma@example.com", Password = "password123" },
                new User { Id = 4, FirstName = "Sara", LastName = "Taylor", Email = "sara.taylor@example.com", Password = "password123" }
            );

            // Seed initial roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin", Description = "Admin Role Description" },
                new Role { Id = 2, Name = "User", Description = "User Role Description" }
            );

            // Seed initial UserRoles
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { UserId = 1, RoleId = 1 }, // John Doe is Admin
                new UserRole { UserId = 2, RoleId = 2 }, // Jane Smith is User
                new UserRole { UserId = 3, RoleId = 1 }, // Hina Sharma is Admin
                new UserRole { UserId = 3, RoleId = 2 }  // Hina Taylor is also User
            );

            // Optionally, seed initial products
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop", Description = "A high-performance laptop.", Price = 1500.00M, Quantity = 100 },
                new Product { Id = 2, Name = "Smartphone", Description = "A latest model smartphone.", Price = 800.00M, Quantity = 50 }
            );
        }

        // DbSet Properties Manage entities: Users, Roles, UserRoles, and Products. 
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}