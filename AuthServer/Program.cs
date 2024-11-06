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
            // Diret�rio de logs na raiz do projeto
            var projectDirectory = @"C:\Users\gusta\source\repos\AuthServer\AuthServer"; // Altere para o caminho base do projeto
            var logDirectory = Path.Combine(projectDirectory, "Logs");

            // Verifica��o e cria��o do diret�rio de logs
            Console.WriteLine($"Verificando diret�rio de logs: {logDirectory}");
            if (!Directory.Exists(logDirectory))
            {
                try
                {
                    Directory.CreateDirectory(logDirectory);
                    Console.WriteLine("Diret�rio de logs criado com sucesso.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Falha ao criar o diret�rio de logs: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Diret�rio de logs j� existe.");
            }

            // Caminho completo do arquivo de log
            var logFilePath = Path.Combine(logDirectory, "application.log");
            Console.WriteLine($"Caminho completo do arquivo de log: {logFilePath}");

            // Configura��o do Serilog para console e arquivo de log
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()  // Console logging
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
                .CreateLogger();

            // Log de inicializa��o
            Log.Information("Aplica��o iniciada - configura��o de logs inicializada.");

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Configura Serilog para o projeto inteiro
                builder.Host.UseSerilog();

                // Configura��o da string de conex�o
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                builder.Services.AddDbContext<AuthServerContext>(options =>
                    options.UseSqlServer(connectionString));

                // Adiciona servi�os ao container
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





                var app = builder.Build();

                // Configura��es de desenvolvimento
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

                // Exce��es personalizadas
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
                Log.Fatal(ex, "A aplica��o falhou ao iniciar.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
