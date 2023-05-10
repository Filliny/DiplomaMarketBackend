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
        private readonly UserManager<UserModel> _userManager;

        public AuthController(BaseContext db, ILogger<AuthController> logger, UserManager<UserModel> userManager)
        {
            _db = db;
            _logger = logger;
            _userManager = userManager;
        }

        /// <summary>
        /// Authenticates user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> MakeAuth([FromBody] RequestAuthModel model)
        {

            var user =  _db.Users.FirstOrDefault(u => u.Email == model.Email.ToLower());

            if(user != null && await _userManager.CheckPasswordAsync(user,model.Password))
            {

                var getJwt = JwtTokenGenerator.GetToken(_userManager,user);
                user.PasswordHash = null;
                var response = new
                {
                    jwt = getJwt,
                    userModel = user
                };
           
                return new JsonResult(response);
   
            }

            return Unauthorized();
        }
    }
}
