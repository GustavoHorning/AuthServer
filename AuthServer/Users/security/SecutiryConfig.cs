using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthServer
{
    public static class SecurityConfig
    {
        public static IServiceCollection AddSecurityConfig(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuração de CORS
            services.AddCors(options =>
            {
                options.AddPolicy("DefaultPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            // Configuração de autenticação JWT
            var jwtSecret = configuration.GetValue<string>("Jwt:Secret") ?? "owp.z;8BhLq(L?2HM(5)u<x)Hg!A[J:h";
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "Auth Server",
                    ValidAudience = "Auth Server",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
                };
            });

            // Configuração de autorização
            services.AddAuthorization();

            return services;
        }
    }
}
