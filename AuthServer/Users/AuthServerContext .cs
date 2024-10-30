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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Mapeando a classe User para a tabela "Users"
            modelBuilder.Entity<User>().ToTable("Users");

            // Configurando o índice único para o campo Email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique(); // Define que o índice deve ser único
        }
    }
}
