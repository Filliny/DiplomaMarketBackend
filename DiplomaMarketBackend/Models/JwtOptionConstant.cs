using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace DiplomaMarketBackend.Models
{
    public class JwtOptionConstant
    {
        public const string Issuer = "OurAspService";
        public const string Audience = "OutClientService";
        private const string SecretKey = "superdupersecretkeyvalue!!!!123321";

        public static SymmetricSecurityKey GetSymmetric()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
        }
    }
}