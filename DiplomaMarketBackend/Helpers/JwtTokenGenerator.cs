using DiplomaMarketBackend.Models;
using Lessons3.Entity.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DiplomaMarketBackend.Helpers
{
    public class JwtTokenGenerator
    {
        public static string GetToken(UserModel model)
        {
            var now = DateTime.UtcNow;
            var jwtSecurity = new JwtSecurityToken(
                JwtOptionConstant.Issuer,
                JwtOptionConstant.Audience,
                notBefore: now,
                claims: GetClaimsIdentity(model).Claims,
                expires: now.AddHours(12),
                signingCredentials: new SigningCredentials(JwtOptionConstant.GetSymmetric(),
                    SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(jwtSecurity);
        }

        private static ClaimsIdentity GetClaimsIdentity(UserModel user)
        {
            if (user == null) return null;

            var claims = new List<Claim>
            {
                new(ClaimsIdentity.DefaultNameClaimType, user.Username),
                new(ClaimsIdentity.DefaultRoleClaimType, user.Role),
                new("hello", "world")
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
    }
}