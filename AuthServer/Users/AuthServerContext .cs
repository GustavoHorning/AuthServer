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

        public DbSet<User> Users { get; set; } // DbSet para a entidade User
        public DbSet<Role> Roles { get; set; } // DbSet para a entidade Roles

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Mapeando a classe User para a tabela "Users"
            modelBuilder.Entity<User>().ToTable("Users");

            // Configurando o índice único para o campo Email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique(); // Define que o índice deve ser único

            //Configuração da entidade Role e índice único para Name
            modelBuilder.Entity<Role>()
            .ToTable("Roles") // Opcional: define o nome da tabela como "Roles"
            .HasIndex(r => r.Name)
            .IsUnique(); // Define que o índice do campo Name deve ser únicomodelBuilder.Entity<Role>()


            // Configurando a relação muitos-para-muitos entre User e Role
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
