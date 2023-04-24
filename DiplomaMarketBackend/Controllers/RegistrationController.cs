using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Helpers;
using Lessons3.Entity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebShopApp.Abstract;

namespace DiplomaMarketBackend.Controllers
{
    [ApiController]
    [Route("authentication/[controller]")]
    public class RegController : ControllerBase
    {
        private readonly ILogger<RegController> _logger;
        private readonly BaseContext _db;
        private readonly IEmailService _emailService;

        public RegController(BaseContext db, ILogger<RegController> logger, IEmailService emailService)
        {
            _db = db;
            _logger = logger;
            _emailService = emailService;
        }

        [HttpPost]
        public IActionResult MakeRegister([FromBody] UserModel model)
        {
            if (model.Username is null || model.Email is null || model.Password is null || model.Role is null)
                return BadRequest(new { registerError = "Some values is null" });

            if (_db.Users.Any(u => u.Email == model.Email.ToLower())) return BadRequest(new { registerError = "User email already exist!" });

            var user = new UserModel
            {
                Username = model.Username,
                Email = model.Email.ToLower(),
                RegDate = DateTime.Now,
                Role = model.Role
            };

            PasswordHasher<UserModel> hasher = new PasswordHasher<UserModel>();
            user.Password = hasher.HashPassword(user, model.Password);


            _db.Users.Add(user);
            _db.SaveChanges();

            var newUserRecord = _db.Users.FirstOrDefault(u => u.Username == user.Username);

            if (newUserRecord != null)
            {
                newUserRecord.Password = null;
                var response = new
                {
                    jwt = JwtTokenGenerator.GetToken(newUserRecord),
                    userModel = newUserRecord
                };

                _emailService.SendEmailAsync(user.Email, "New registration", $"<p>Your password is {model.Password}</p>");
                return new JsonResult(response);
            }

            return BadRequest(new { authError = "User create error" });


        }

        [HttpPost]
        public IActionResult PasswordRecovery([FromQuery] string email)
        {
            var  user  = _db.Users.FirstOrDefault(u => u.Email == email); 

            if (user != null)
            {
                
            }

            return Ok();
        }
    }
}
    

