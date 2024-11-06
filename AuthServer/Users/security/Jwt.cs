//using AuthServer.Users;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using Newtonsoft.Json;


//public class Jwt
//{
//    private const string ISSUER = "Auth Server";
//    private const string SECRET = "owp.z;8BhLq(L?2HM(5)u<x)Hg!A[J:h";
//    private const int EXPIRE_HOURS = 24;

//    public string CreateToken(User user)
//    {
//        var claims = new List<Claim>
//    {
//        new Claim(JwtRegisteredClaimNames.Sub, user.ID.ToString()),
//        new Claim("user", Newtonsoft.Json.JsonConvert.SerializeObject(new UserToken(user.ID, user.Name, user.Roles.Select(r => r.Name).ToHashSet())))
//    };

//        // Adicionando roles como claims (opcional, dependendo do que você deseja incluir)
//        foreach (var role in user.Roles)
//        {
//            claims.Add(new Claim(ClaimTypes.Role, role.Name));
//        }

//        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(SECRET));
//        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//        var token = new JwtSecurityToken(
//            issuer: ISSUER,
//            audience: ISSUER,
//            claims: claims,
//            expires: DateTime.Now.AddHours(EXPIRE_HOURS),
//            notBefore: DateTime.Now, // Define a hora em que o token começa a ser válido
//            signingCredentials: creds);

//        return new JwtSecurityTokenHandler().WriteToken(token);
//    }


//    public ClaimsPrincipal? Extract(string token)
//    {
//        try
//        {
//            var tokenHandler = new JwtSecurityTokenHandler();
//            var key = System.Text.Encoding.UTF8.GetBytes(SECRET);

//            var validationParameters = new TokenValidationParameters
//            {
//                ValidateIssuer = true,
//                ValidateAudience = true,
//                ValidateIssuerSigningKey = true,
//                ValidIssuer = ISSUER,
//                ValidAudience = ISSUER,
//                IssuerSigningKey = new SymmetricSecurityKey(key)
//            };

//            // Valida o token e extrai o ClaimsPrincipal
//            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

//            return principal;
//        }
//        catch (Exception)
//        {
//            // Token inválido ou erro de validação
//            return null;
//        }
//    }

//}

using AuthServer.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json;

namespace AuthServer.Security
{
    public class Jwt
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

