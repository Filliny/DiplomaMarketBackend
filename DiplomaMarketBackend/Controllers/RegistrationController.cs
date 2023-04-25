using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Helpers;
using DiplomaMarketBackend.Models;
using Lessons3.Entity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;
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
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public RegController(BaseContext db, ILogger<RegController> logger, IEmailService emailService, UserManager<UserModel> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _db = db;
            _logger = logger;
            _emailService = emailService;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("register-user")]
        public async Task<IActionResult> UserRegister([FromBody] User model)
        {

            if (model.Username is null || model.Email is null || model.Password is null)
                return BadRequest(new { registerError = "Some values is null" });


            var userExists = await _userManager.FindByNameAsync(model.Username);

            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User already exists!" });

            UserModel user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            var role_result = await _userManager.AddToRoleAsync(user, "User");

            if (!result.Succeeded || !role_result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new  { Status = "Error", Message = "User creation failed! Please check user details and try again." });


            user.PasswordHash = null;
            var response = new
            {
                jwt = JwtTokenGenerator.GetToken(_userManager, user),
                userModel = user,
                Status = "Success",
                Message = "User created successfully!"

            };

            await _emailService.SendEmailAsync(user.Email, "New registration", $"<p>Your password is {model.Password}</p>");

            return new JsonResult(response);


        }


        [HttpPost]
        [Authorize(Roles ="Admin")]
        [Route("register-admin")]
        public async Task<IActionResult> AdminRegister([FromBody] User model, string role)
        {

            if (model.Username is null || model.Email is null || model.Password is null || role is null)
                return BadRequest(new { registerError = "Some values is null" });


            var userExists = await _userManager.FindByNameAsync(model.Username);

            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User already exists!" });

            UserModel user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                EmailConfirmed = true
            };

            if (!await _roleManager.RoleExistsAsync(role))
                 return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User creation failed! Role not exist." });

            var result = await _userManager.CreateAsync(user, model.Password);
            var role_result = await _userManager.AddToRoleAsync(user, role);

            if (!result.Succeeded || !role_result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User creation failed! Please check user details and try again." });


            user.PasswordHash = null;
            var response = new
            {
                jwt = JwtTokenGenerator.GetToken(_userManager, user),
                userModel = user,
                Status = "Success",
                Message = "User created successfully!"

            };

            return new JsonResult(response);


        }

        [HttpPost]
        [Route("pass-recovery")]
        public async Task<IActionResult> PasswordRecovery([FromQuery] string email)
        {
            var user = await _userManager.FindByEmailAsync(email.ToLower());

            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return BadRequest("User is not exist or email not confirmed!");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                    "/Account/ResetPassword", //todo set right callback
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

            await _emailService.SendEmailAsync(email,"Reset Password",
                 $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl??"")}'>clicking here</a>.");

            return Ok();
        }

        [HttpPost]
        [Route("recovery_confirm")]
        public async Task<IActionResult> PasswordRecoveryConfirm([FromQuery] string email,string new_password,string email_code)
        {
            var user = await _userManager.FindByEmailAsync(email.ToLower());

            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return BadRequest("User is not exist or email not confirmed!");
            }

            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(email_code));
            var result = await _userManager.ResetPasswordAsync(user, code, new_password);

            if (!result.Succeeded) return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Password change error: " + result.ToString() });

            return Ok();
        }
    }
}
    

