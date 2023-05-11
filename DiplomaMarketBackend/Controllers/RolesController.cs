using DiplomaMarketBackend.Entity;
using Lessons3.Entity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WebShopApp.Abstract;

namespace DiplomaMarketBackend.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : Controller
    {

        private readonly ILogger<RegController> _logger;
        private readonly BaseContext _context;
        private readonly IEmailService _emailService;
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public RolesController(BaseContext context, ILogger<RegController> logger, IEmailService emailService, UserManager<UserModel> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _emailService = emailService;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        /// <summary>
        /// Get all available names of roles
        /// </summary>
        /// <returns>List of role names</returns>
        [HttpGet]
        [Route("roles")]
        public async Task<IActionResult> Get()
        {
            var roles = await _roleManager.Roles.Select(r=>r.Name).ToListAsync();

            return new JsonResult(roles);
        }

        /// <summary>
        /// Create role 
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <returns>Ok if sucess</returns>
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateRole([FromForm] string roleName)
        {
            var role = new IdentityRole { Name = roleName };

            try
            {
                await _roleManager.CreateAsync(role);
            }
            catch (Exception ex )
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { Satus = "Error", Message = ex.Message });
            }

            return Ok(role);

        }


        /// <summary>
        /// Update existing role by name
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <param name="newName">New role name</param>
        /// <returns></returns>
        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> UpdateRole([FromForm] string  roleName, string newName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            try
            {
                if (role == null) { throw new Exception("Role not found! "); }

                role.Name = newName;
                await _roleManager.UpdateAsync(role);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { Satus = "Error", Message = ex.Message });
            }

            return Ok(role);

        }

        /// <summary>
        /// Delete role by name
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <returns>Ok if sucess</returns>
        /// <response code="500">If role deleting unsuccessfull</response>
        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteRole([FromForm] string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            try
            {
                if (role == null) { throw new Exception("Role not found! "); }

                await _roleManager.DeleteAsync(role);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { Satus = "Error", Message = ex.Message });
            }

            return Ok(role);
        }
    }
}
