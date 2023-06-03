using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Helpers;
using DiplomaMarketBackend.Models;
using Lessons3.Entity.Models;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DiplomaMarketBackend.Controllers
{
    [ApiController]
    [Route("authentication/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly BaseContext _context;
        private readonly UserManager<UserModel> _userManager;

        public AuthController(BaseContext context, ILogger<AuthController> logger, UserManager<UserModel> userManager)
        {
            _context = context;
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
            var user = await _userManager.FindByNameAsync(model.Email);

            if (user != null)
            {
                if (await _userManager.CheckPasswordAsync(user, model.Password))
                {

                    var user_dto = user.Adapt<UserFull>();
                    var role = await _userManager.GetRolesAsync(user);

                    if (role != null)
                    {
                        user_dto.roles = role;
                    }

                    var getJwt = JwtTokenGenerator.GetToken(_userManager, user);
                    user.PasswordHash = null;
                    var response = new
                    {
                        jwt = getJwt,
                        user = user_dto
                    };

                    return new JsonResult(response);
                }

                return Unauthorized("Check user password!");
            }

            return Unauthorized("Check user login!");
        }

    }
}
