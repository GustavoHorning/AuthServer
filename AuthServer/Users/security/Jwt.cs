using AuthServer.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json;

namespace AuthServer.Security
{

    public interface IJwt
    {
        public string CreateToken(User user);
        public ClaimsPrincipal? Extract(string token);
    }
    public class Jwt : IJwt
    {
        private readonly SecuritySettings _securitySettings;

        public Jwt(IOptions<SecuritySettings> securitySettings)
        {
            _securitySettings = securitySettings.Value;
        }

        public string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.ID.ToString()),
                new Claim("user", JsonConvert.SerializeObject(new UserToken(user.ID, user.Name, user.Roles.Select(r => r.Name).ToHashSet())))
            };

            // Adicionando roles como claims
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securitySettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _securitySettings.Issuer,
                audience: _securitySettings.Issuer,
                claims: claims,
                expires: DateTime.Now.AddHours(_securitySettings.ExpireHours),
                notBefore: DateTime.Now,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? Extract(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_securitySettings.Secret);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _securitySettings.Issuer,
                    ValidAudience = _securitySettings.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                // Valida o token e extrai o ClaimsPrincipal
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

                return principal;
            }
            catch (Exception)
            {
                // Token inválido ou erro de validação
                return null;
            }
        }
    }
}

