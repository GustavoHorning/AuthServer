using AuthServer.Users;
using Microsoft.EntityFrameworkCore;

namespace AuthServer
{
    public class AuthServerContext : DbContext
    {
        public AuthServerContext(DbContextOptions<AuthServerContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } 
        public DbSet<Role> Roles { get; set; } 




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users");

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Role>()
            .ToTable("Roles") 
            .HasIndex(r => r.Name)
            .IsUnique(); 


            
            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    j => j
                        .HasOne<Role>()
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK_UserRole_RoleId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<User>()
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_UserRole_UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                );
        }
    }
}
