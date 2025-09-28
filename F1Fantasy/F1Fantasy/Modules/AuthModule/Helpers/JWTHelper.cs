using F1Fantasy.Core.Common;
using F1Fantasy.Core.Configurations;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace F1Fantasy.Modules.AuthModule.Helpers
{
    public class JWTHelper
    {
        private static string GenerateJwtToken(ApplicationUser user, AuthConfiguration authConfiguration, string userRole, IList<Claim> roleClaims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authConfiguration.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = [
                new(JwtRegisteredClaimNames.Sub, user.Email!),
                new("userid", user.Id.ToString()),
                new("role", userRole)
            ];

            foreach (var roleClaim in roleClaims)
            {
                claims.Add(new Claim(roleClaim.Type, roleClaim.Value));
            }

            var token = new JwtSecurityToken(
                issuer: authConfiguration.Issuer,
                audience: authConfiguration.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(authConfiguration.TokenValidityInHours),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}