using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Helpers;
using DiplomaMarketBackend.Models;
using Lessons3.Entity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DiplomaMarketBackend.Controllers
{
    [ApiController]
    [Route("authentication/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly BaseContext _db;

        public AuthController(BaseContext db, ILogger<AuthController> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// Authenticates user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult MakeAuth([FromBody] RequestAuthModel model)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == model.Email.ToLower());

            if(user != null)
            {
                PasswordHasher<UserModel> hasher = new PasswordHasher<UserModel>();

                if(hasher.VerifyHashedPassword(user,user.Password,model.Password) == PasswordVerificationResult.Success)
                {
                    var getJwt = JwtTokenGenerator.GetToken(user);
                    user.Password = null;
                    var response = new
                    {
                        jwt = getJwt,
                        userModel = user
                    };

                    return new JsonResult(response);
                }

                return BadRequest(new { authError = "Error credentials" });

            }

            return BadRequest(new { authError = "User not found" });
        }
    }
}
