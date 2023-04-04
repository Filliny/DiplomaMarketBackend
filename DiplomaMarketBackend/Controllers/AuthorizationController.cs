using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Helpers;
using DiplomaMarketBackend.Models;
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
        /// Autentificates user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult MakeAuth([FromBody] RequestAuthModel model)
        {
            var user = _db.Users.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);

            if (user is null) return BadRequest(new { authError = "Error credentials" });

            var getJwt = JwtTokenGenerator.GetToken(user);
            user.Password = null;
            var response = new
            {
                jwt = getJwt,
                userModel = user
            };

            return new JsonResult(response);
        }
    }
}
