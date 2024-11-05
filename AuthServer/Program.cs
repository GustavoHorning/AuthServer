using AuthServer.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

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
            builder.Services.AddAuthentication();
            builder.Services.AddSecurityConfig(builder.Configuration);
            builder.Services.AddSingleton<JwtTokenFilter>(); // Registrando o middleware como serviço
            builder.Services.AddTransient<Jwt>(); // Registra o serviço Jwt
            builder.Services.AddTransient<UsersService>();
            builder.Services.AddTransient<UsersRepository>(); // Utilize o UsersRepository como um serviço
            builder.Services.AddHostedService<UsersBootstrap>();
            builder.Services.AddTransient<RoleRepository>();
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<JwtTokenFilter>();
            app.UseHttpsRedirection();
            app.UseCors("DefaultPolicy"); // Aplicando política de CORS
            app.UseAuthentication(); // Middleware para autenticação
            app.UseAuthorization();  // Middleware para autorização
            app.MapControllers();

            app.Run();
        }
    }
}
