using AuthServer.Users;
using Microsoft.EntityFrameworkCore;

namespace AuthServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configurando a string de conex�o
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AuthServerContext>(options =>
                options.UseSqlServer(connectionString)); // Adiciona o DbContext com a string de conex�o

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Registrando UsersRepository e UsersService como servi�os
            builder.Services.AddTransient<UsersService>();
            builder.Services.AddTransient<UsersRepository>(); // Utilize o UsersRepository como um servi�o

            // Adicione o UsersBootstrap ao pipeline de servi�os
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
