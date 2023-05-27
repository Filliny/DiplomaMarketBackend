using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Models;
using DiplomaMarketBackend.Models;
using Lessons3.Entity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DiplomaMarketBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : Controller
    {

        private readonly ILogger<RolesController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly BaseContext _context;
        private readonly UserManager<UserModel> _userManager;


        public RolesController(ILogger<RolesController> logger, UserManager<UserModel> userManager, BaseContext context,
            RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _roleManager = roleManager;
            _context = context;
            _userManager = userManager;
        }


        /// <summary>
        /// Get existing roles (customer groups)
        /// </summary>
        /// <returns>List of role names with id</returns>
        [Authorize(Roles = "CustomersRead")]
        [HttpGet]
        [Route("roles")]
        public async Task<IActionResult> Get()
        {
            var roles = await _context.CustomerGroups.Select(g => new { id = g.Id, name = g.Name }).ToListAsync();

            return new JsonResult(roles);
        }


        /// <summary>
        /// Get existing permissions
        /// </summary>
        /// <returns>List of permission names with id</returns>
        [Authorize(Roles = "CustomersRead")]
        [HttpGet]
        [Route("permissions")]
        public async Task<IActionResult> Permissions()
        {
            var permissions = await _context.Permissions.Select(g => new { id = g.Id, name = g.Description }).ToListAsync();

            return new JsonResult(permissions);
        }


        /// <summary>
        /// Get users role and assigned permissions settings
        /// </summary>
        /// <param name="role_id"></param>
        /// <returns>Ok if sucess</returns>
        [Authorize(Roles = "CustomersRead")]
        [HttpGet]
        [Route("get-group")]
        public async Task<ActionResult<CustomersRole>> GetRole([FromQuery] int role_id)
        {
            var customer_group = await _context.CustomerGroups.
                Include(r=>r.PermissionsKeys).ThenInclude(k=>k.Permission).
                FirstOrDefaultAsync(r => r.Id == role_id);

            if (customer_group == null) return BadRequest(new Result
            {
                Status = "Error",
                Message = "Users group not found!"
            });

            var response = new CustomersRole
            {
                id = customer_group.Id,
                name = customer_group.Name,
            };

            var  permissions_group = customer_group.PermissionsKeys.GroupBy(p=>p.Permission).ToList();

            //test gen
            //var perrmissions = _context.PermissionKeys.Include(k => k.Permission).GroupBy(k => k.Permission).ToList();

            foreach(var permission in permissions_group)
            {
                var prem = new CustomersRole.SelectedPermission
                {
                    id = permission.Key.Id,
                    name = permission.Key.Name,
                    read_allowed = true,
                    write_allowed = true,
                    
                };

                foreach(var key in permission)
                {
                    if(key.Allowed == AllowedKey.read) prem.read_allowed = true;
                    if (key.Allowed == AllowedKey.write) prem.write_allowed = true;

                }

                response.permissions.Add(prem);
            }

            return Ok(response);

        }

        /// <summary>
        /// Create roles customers group  
        /// </summary>
        /// <param name="customers_group"></param>
        /// <returns>Ok if sucess</returns>
        [Authorize(Roles = "CustomersWrite")]
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateRole([FromBody] CustomersRole customers_group)
        {
            var new_group = new CustomerGroupModel 
            {
                Name = customers_group.name,
            };

            var permissions_list = await _context.Permissions.Include(p=>p.Keys).ToListAsync();

            foreach(var permission in  customers_group.permissions)
            {
                var base_permission = permissions_list.FirstOrDefault(p => p.Id == permission.id);
                if(base_permission == null) continue;

                if (permission.read_allowed) new_group.PermissionsKeys.Add(base_permission.Keys.First(k => k.Allowed == AllowedKey.read));
                if (permission.write_allowed) new_group.PermissionsKeys.Add(base_permission.Keys.First(k => k.Allowed == AllowedKey.write));
            }

            _context.CustomerGroups.Add(new_group);
            _context.SaveChanges();

            return Ok(new Result
            {
                Status = "Success",
                Message = "Customers group succesfully added",
                Entity = customers_group

            });
        }

        /// <summary>
        /// Update existing role group
        /// for "permissions" list can be sended only changed permission row
        /// </summary>
        /// <param name="customers_group"></param>
        /// <returns>Ok if success</returns>
        [Authorize(Roles = "CustomersWrite")]
        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> UpdateRole([FromBody] CustomersRole customers_group)
        {
            var exist_group = await _context.CustomerGroups.Include(g=>g.PermissionsKeys).FirstOrDefaultAsync(g=>g.Id == customers_group.id);
            if (exist_group == null) return BadRequest(new Result
            {
                Status = "Error",
                Message = "Group not found",
                Entity = customers_group
            });

            exist_group.Name = customers_group.name;

            var permissions_list = await _context.Permissions.Include(p => p.Keys).ToListAsync();

            foreach (var permission in customers_group.permissions)
            {
                //remove existing permissions
                var to_remove = exist_group.PermissionsKeys.Where(k => k.PermissionId == permission.id);
                
                foreach(var key in to_remove)
                {
                    exist_group.PermissionsKeys.Remove(key);
                }

                //readd any of changed
                var base_permission = permissions_list.FirstOrDefault(p => p.Id == permission.id);
                if (base_permission == null) continue;

                if (permission.read_allowed) exist_group.PermissionsKeys.Add(base_permission.Keys.First(k => k.Allowed == AllowedKey.read));
                if (permission.write_allowed) exist_group.PermissionsKeys.Add(base_permission.Keys.First(k => k.Allowed == AllowedKey.write));

            }

            _context.CustomerGroups.Update(exist_group);
            _context.SaveChanges();

            return Ok(new Result
            {
                Status = "Success",
                Message = "Customers group succesfully updated",
                Entity = customers_group

            });

        }

        /// <summary>
        /// Delete role by name
        /// </summary>
        /// <param name="group_id">Group Id</param>
        /// <returns>Ok if sucess</returns>
        /// <response code="00">If group is not exist</response>
        /// <response code="500">If role deleting unsuccessfull (have users related)</response>
        [Authorize(Roles = "CustomersWrite")]
        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteRole([FromQuery] int group_id)
        {
            var exist_group = await _context.CustomerGroups.Include(g => g.Customers).FirstOrDefaultAsync(g => g.Id == group_id);
            if (exist_group == null) return BadRequest(new Result
            {
                Status = "Error",
                Message = "Group not found",

            });

            try
            {
                if (exist_group.Customers.Count != 0) { throw new Exception("Cant delete group with customers relaying! "); }

                _context.CustomerGroups.Remove(exist_group);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { Satus = "Error", Message = ex.Message });
            }

            return Ok(new Result
            {
                Status = "Success",
                Message = "Customers group succesfully deleted",
                Entity = exist_group

            });
        }
    }
}
