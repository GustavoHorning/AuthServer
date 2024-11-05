using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

public class JwtTokenFilter : IMiddleware
{
    private readonly Jwt _jwtService;

    public JwtTokenFilter(Jwt jwtService)
    {
        _jwtService = jwtService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

        if (authHeader != null && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            var principal = _jwtService.Extract(token);

            if (principal != null)
            {
                context.User = principal;
            }
        }

        await next(context);
    }
}
