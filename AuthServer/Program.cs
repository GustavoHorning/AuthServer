using AuthServer.Security;
using AuthServer.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.IO;
using System.Text;

namespace AuthServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var projectDirectory = @"C:\Users\gusta\source\repos\AuthServer\AuthServer";
            var logDirectory = Path.Combine(projectDirectory, "Logs");

            Console.WriteLine($"Verificando diretório de logs: {logDirectory}");
            if (!Directory.Exists(logDirectory))
            {
                try
                {
                    Directory.CreateDirectory(logDirectory);
                    Console.WriteLine("Diretório de logs criado com sucesso.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Falha ao criar o diretório de logs: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Diretório de logs já existe.");
            }

            var logFilePath = Path.Combine(logDirectory, "application.log");
            Console.WriteLine($"Caminho completo do arquivo de log: {logFilePath}");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()  
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
                .CreateLogger();

            Log.Information("Aplicação iniciada - configuração de logs inicializada.");

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog();

                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                builder.Services.AddDbContext<AuthServerContext>(options =>
                    options.UseSqlServer(connectionString));

                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();
                builder.Services.AddAuthentication();
                builder.Services.AddSecurityConfig(builder.Configuration);
                builder.Services.AddSingleton<JwtTokenFilter>();
                builder.Services.AddTransient<Jwt>();
                builder.Services.AddTransient<UsersService>();
                builder.Services.AddTransient<UsersRepository>();
                builder.Services.AddHostedService<UsersBootstrap>();
                builder.Services.AddTransient<RoleRepository>();
                builder.Services.Configure<SecuritySettings>(builder.Configuration.GetSection("Security"));
                builder.Services.AddSingleton<Jwt>();
                builder.Services.AddScoped<IUsersRepository, UsersRepository>();
                builder.Services.AddScoped<IRoleRepository, RoleRepository>();
                builder.Services.AddScoped<IJwt, Jwt>();






                var app = builder.Build();

                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseMiddleware<JwtTokenFilter>();
                app.UseHttpsRedirection();
                app.UseCors("DefaultPolicy");
                app.UseAuthentication();
                app.UseAuthorization();
                app.MapControllers();

                app.Use(async (context, next) =>
                {
                    try
                    {
                        await next();
                    }
                    catch (BadRequestException ex)
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync(ex.Message);
                    }
                });

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "A aplicação falhou ao iniciar.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
