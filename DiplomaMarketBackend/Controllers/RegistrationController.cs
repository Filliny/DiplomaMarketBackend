using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Helpers;
using Lessons3.Entity.Models;
using Microsoft.AspNetCore.Mvc;

namespace DiplomaMarketBackend.Controllers
{
    [ApiController]
    [Route("authentication/[controller]")]
    public class RegController : ControllerBase
    {
        private readonly ILogger<RegController> _logger;
        private readonly BaseContext _db;

        public RegController(BaseContext db, ILogger<RegController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult MakeRegister([FromBody] UserModel model)
        {
            if (model.Username is null || model.Email is null || model.Password is null || model.Role is null)
                return BadRequest(new { registerError = "Some values is null" });

            var user = new UserModel
            {
                Username = model.Username,
                Password = model.Password,
                Email = model.Email,
                RegDate = DateTime.Now,
                Role = model.Role
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            var newUserRecord = _db.Users.FirstOrDefault(u => u.Username == user.Username);
            newUserRecord.Password = null;
            var response = new
            {
                jwt = JwtTokenGenerator.GetToken(newUserRecord),
                userModel = newUserRecord
            };
            return new JsonResult(response);
        }
    }
}
    

