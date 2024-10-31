using AuthServer.Users;
using Microsoft.EntityFrameworkCore;

namespace AuthServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configurando a string de conexão
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AuthServerContext>(options =>
                options.UseSqlServer(connectionString)); // Adiciona o DbContext com a string de conexão

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Registrando UsersRepository e UsersService como serviços
            builder.Services.AddTransient<UsersService>();
            builder.Services.AddTransient<UsersRepository>(); // Utilize o UsersRepository como um serviço

            // Adicione o UsersBootstrap ao pipeline de serviços
            builder.Services.AddHostedService<UsersBootstrap>();
            builder.Services.AddTransient<RoleRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
