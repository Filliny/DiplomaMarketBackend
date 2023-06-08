using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Helpers;
using DiplomaMarketBackend.Models;
using DiplomaMarketBackend.Entity.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
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


		public RegController(BaseContext db, ILogger<RegController> logger, IEmailService emailService, UserManager<UserModel> userManager,
			RoleManager<IdentityRole> roleManager
			)
		{
			_db = db;
			_logger = logger;
			_emailService = emailService;
			_userManager = userManager;
			_roleManager = roleManager;
		}
		
		/// <summary>
		/// Register user from register customer form
		/// </summary>
		/// <param name="model">User model</param>
		/// <returns>Ok if success with status and JWT key</returns>
		[HttpPost]
		[Route("register-user")]
		public async Task<IActionResult> UserRegister([FromBody] User model)
		{

			if (model.user_name is null || model.email is null || model.password is null)
				return BadRequest(new Result { 
					Status = "Error",
					Message = "Some values is null",
					Entity = model
				});


			var userExists = await _userManager.FindByNameAsync(model.email);

			if (userExists != null)
				return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User already exists!" });

			UserModel user = new()
			{
				Email = model.email,
				SecurityStamp = Guid.NewGuid().ToString(),
				UserName = model.email,
				EmailConfirmed = true,
				RegDate = DateTime.Now,
			};

			var result = await _userManager.CreateAsync(user, model.password);
			var role_result = await _userManager.AddToRoleAsync(user, "User");

			if (!result.Succeeded || !role_result.Succeeded)
				return StatusCode(StatusCodes.Status500InternalServerError, new  { Status = "Error", Message = "User creation failed! Please check user details and try again." });


            var user_dto = user.Adapt<UserFull>();
            var role = await _userManager.GetRolesAsync(user);

            //if (role != null)
            //{
            //    user_dto.roles = role;
            //}

            user.PasswordHash = null;
			var response = new
			{
				jwt = JwtTokenGenerator.GetToken(_userManager, user),
				userModel = user_dto,
				Status = "Success",
				Message = "User created successfully!"

			};

			await _emailService.SendEmailAsync(user.Email, "New registration", $"<p>Your password is {model.password}</p>");

			return new JsonResult(response);

		}

		/// <summary>
        /// Password recovery send mail endpoint
        /// </summary>
        /// <param name="email">email to send recovery code</param>
        /// <param name="front_callback">Frontend callback for email confirm route e.g. http://your_host/confirm_email</param>
        /// <returns>Ok if send succesfull</returns>
        /// <response code="400">If user with given email is not exist</response>
        [HttpPost]
		[Route("pass-recovery")]
		public async Task<IActionResult> PasswordRecovery([FromQuery] string email, string front_callback)
		{
			var user = await _userManager.FindByEmailAsync(email.ToLower());

			if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
			{
				return BadRequest("User is not exist or email not confirmed!");
			}

			var code = await _userManager.GeneratePasswordResetTokenAsync(user);
			code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
			var callbackUrl = $"{front_callback}?email={email}&code={code}";

			await _emailService.SendEmailAsync(email, "Reset Password",
			 $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl ?? "")}'>clicking here</a>.");

			return Ok(new Result
			{
				Status = "Success",
				Message = "Password recovery request successfully created ",
				Entity = new
				{
					code,
					callbackUrl,
					email
				}
			});
		}

		/// <summary>
		/// Password recovery confirm endpoint
		/// </summary>
		/// <param name="email">User Email</param>
		/// <param name="new_password">New Password</param>
		/// <param name="email_code">code from email</param>
		/// <returns></returns>
		[HttpPost]
		[Route("recovery_confirm")]
		public async Task<IActionResult> PasswordRecoveryConfirm([FromForm] string email, [FromForm]  string new_password, [FromForm]  string email_code)
		{
			var user = await _userManager.FindByEmailAsync(email.ToLower());

			if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
			{
				return BadRequest("User is not exist or email not confirmed!");
			}
			
			var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(email_code));
			var result = await _userManager.ResetPasswordAsync(user, code, new_password);

			if (!result.Succeeded) return StatusCode(StatusCodes.Status500InternalServerError,
				new { Status = "Error", Message = "Password change error: " + result.ToString() });

			return Ok();
		}

		/// <summary>
		/// Update user password
		/// </summary>
		/// <param name="old_password">Old (current) user password</param>
		/// <param name="new_password">New password</param>
		/// <returns>Ok if success</returns>
		[Authorize]
		[HttpPost]
		[Route("pass-update")]
		public async Task<IActionResult> PasswordUpdate([FromForm] string old_password, [FromForm] string new_password)
		{
			try
			{
				var user = await _userManager.FindByNameAsync(User.Identity.Name);

				if (user ==  null || !(await _userManager.HasPasswordAsync(user)))
				{
					throw new Exception("User not found or havent password set!");
				}
					 
				await _userManager.ChangePasswordAsync(user,old_password,new_password);

				return Ok(new Result
				{
					Status="Success",
					Message ="Update password success"
				});
			}
			catch( Exception ex)
			{
				return BadRequest(new Result
				{
					Status = "Error",
					Message = "Update password error: " + ex.Message
				});
			}
		}

	}
}
	

