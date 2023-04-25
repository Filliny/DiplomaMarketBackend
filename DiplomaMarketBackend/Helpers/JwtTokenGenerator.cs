using DiplomaMarketBackend.Models;
using Lessons3.Entity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DiplomaMarketBackend.Helpers
{
    public class JwtTokenGenerator
    {
        public static string GetToken(UserManager<UserModel> userManager, UserModel model)
        {
            var now = DateTime.UtcNow;
            var jwtSecurity = new JwtSecurityToken(
                JwtOptionConstant.Issuer,
                JwtOptionConstant.Audience,
                notBefore: now,
                claims: GetClaimsIdentity(userManager, model).Result.Claims,
                expires: now.AddHours(12),
                signingCredentials: new SigningCredentials(JwtOptionConstant.GetSymmetric(),
                    SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(jwtSecurity);
        }

        private async static Task<ClaimsIdentity> GetClaimsIdentity(UserManager<UserModel> userManager,UserModel user)
        {
            if (user == null) return null;

            var roles = await userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            };

            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            return claimsIdentity;
        }
    }
}