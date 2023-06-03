using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Models;
using DiplomaMarketBackend.Models;
using DiplomaMarketBackend.Entity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;

namespace DiplomaMarketBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class GroupsController : Controller
    {

        private readonly ILogger<GroupsController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly BaseContext _context;
        private readonly UserManager<UserModel> _userManager;


        public GroupsController(ILogger<GroupsController> logger, UserManager<UserModel> userManager, BaseContext context,
            RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _roleManager = roleManager;
            _context = context;
            _userManager = userManager;
        }


        /// <summary>
        /// Get existing user groups
        /// </summary>
        /// <returns>List of role names with id</returns>
        [Authorize(Roles = "CustomersRead,Admin")]
        [HttpGet]
        [Route("groups")]
        public async Task<IActionResult> Get()
        {
            var roles = await _context.CustomerGroups.
                Select(g => new
                {
                    id = g.Id,
                    name = g.Name,
                    creation_date = g.Created.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                    state = g.State,
                }).ToListAsync();

            return new JsonResult(roles);
        }


        /// <summary>
        /// Get existing permissions
        /// </summary>
        /// <returns>List of permission names with id</returns>
        [Authorize(Roles = "CustomersRead,Admin")]
        [HttpGet]
        [Route("permissions")]
        public async Task<IActionResult> Permissions()
        {
            var permissions = await _context.Permissions.Select(g => new { id = g.Id, name = g.Description }).ToListAsync();

            return new JsonResult(permissions);
        }


        /// <summary>
        /// Get users group and assigned permissions settings
        /// </summary>
        /// <param name="group_id"></param>
        /// <returns>Group data and existing permissions settings</returns>
        [Authorize(Roles = "CustomersRead,Admin")]
        [HttpGet]
        [Route("get-group")]
        public async Task<ActionResult<CustomersRole>> GetRole([FromQuery] int group_id)
        {
            var customer_group = await _context.CustomerGroups.
                Include(r => r.PermissionsKeys).ThenInclude(k => k.Permission).
                FirstOrDefaultAsync(r => r.Id == group_id);

            if (customer_group == null) return BadRequest(new Result
            {
                Status = "Error",
                Message = "Users group not found!"
            });

            var response = new CustomersRole
            {
                id = customer_group.Id,
                name = customer_group.Name,
                state = customer_group.State,
            };

            var permissions_group = customer_group.PermissionsKeys.GroupBy(p => p.Permission).ToList();

            //test gen
            //var perrmissions = _context.PermissionKeys.Include(k => k.Permission).GroupBy(k => k.Permission).ToList();

            foreach (var permission in permissions_group)
            {
                var prem = new CustomersRole.SelectedPermission
                {
                    id = permission.Key.Id,
                    name = permission.Key.Name,
                    read_allowed = true,
                    write_allowed = true,

                };

                foreach (var key in permission)
                {
                    if (key.Allowed == AllowedKey.read) prem.read_allowed = true;
                    if (key.Allowed == AllowedKey.write) prem.write_allowed = true;

                }

                response.permissions.Add(prem);
            }

            return Ok(response);

        }

        /// <summary>
        /// Create user group  
        /// </summary>
        /// <param name="user_group"></param>
        /// <returns>Ok if sucess</returns>
        [Authorize(Roles = "CustomersWrite,Admin")]
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateRole([FromBody] CustomersRole user_group)
        {
            var new_group = new CustomerGroupModel
            {
                Name = user_group.name,
                Created = DateTime.Now,
            };

            var permissions_list = await _context.Permissions.Include(p => p.Keys).ToListAsync();

            foreach (var permission in user_group.permissions)
            {
                var base_permission = permissions_list.FirstOrDefault(p => p.Id == permission.id);
                if (base_permission == null) continue;

                if (permission.read_allowed) new_group.PermissionsKeys.Add(base_permission.Keys.First(k => k.Allowed == AllowedKey.read));
                if (permission.write_allowed) new_group.PermissionsKeys.Add(base_permission.Keys.First(k => k.Allowed == AllowedKey.write));
            }

            _context.CustomerGroups.Add(new_group);
            _context.SaveChanges();
            user_group.id = new_group.Id;

            return Ok(new Result
            {
                Status = "Success",
                Message = "Customers group succesfully added",
                Entity = user_group
            });
        }

        /// <summary>
        /// Update existing user group
        /// for "permissions" list can be sended only changed permission row
        /// </summary>
        /// <param name="user_group"></param>
        /// <returns>Ok if success</returns>
        [Authorize(Roles = "CustomersWrite,Admin")]
        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> UpdateRole([FromBody] CustomersRole user_group)
        {
            var exist_group = await _context.CustomerGroups.Include(g => g.PermissionsKeys).FirstOrDefaultAsync(g => g.Id == user_group.id);
            if (exist_group == null) return BadRequest(new Result
            {
                Status = "Error",
                Message = "Group not found",
                Entity = user_group
            });

            exist_group.Name = user_group.name;

            var permissions_list = await _context.Permissions.Include(p => p.Keys).ToListAsync();
            var control_sum = exist_group.PermissionsKeys.Sum(k => k.Id);

            foreach (var permission in user_group.permissions)
            {
                //suggesting that we have sended ONLY changed permissions lines - remove responding existing permissions keys before adding new
                var to_remove = exist_group.PermissionsKeys.Where(k => k.PermissionId == permission.id).ToList();

                foreach (var key in to_remove)
                {
                    exist_group.PermissionsKeys.Remove(key);
                }

                //readd any of changed
                var base_permission = permissions_list.FirstOrDefault(p => p.Id == permission.id);
                if (base_permission == null) continue;

                if (permission.read_allowed) exist_group.PermissionsKeys.Add(base_permission.Keys.First(k => k.Allowed == AllowedKey.read));
                if (permission.write_allowed) exist_group.PermissionsKeys.Add(base_permission.Keys.First(k => k.Allowed == AllowedKey.write));

                //update users of group

            }
            var result_sum = exist_group.PermissionsKeys.Sum(k => k.Id);

            //if any changes - update users

            var users = await _context.Users.Where(u => u.CustomerGroupId == exist_group.Id).ToListAsync();

            foreach (var user in users)
            {
                //process state
                if (exist_group.State.Equals("active"))
                    await _userManager.SetLockoutEnabledAsync(user, false);
                else if (exist_group.State.Equals("inactive"))
                    await _userManager.SetLockoutEnabledAsync(user, true);

                if (control_sum != result_sum)
                {

                    //process roles
                    var user_roles = await _userManager.GetRolesAsync(user);

                    var new_roles = new List<string>();
                    foreach (var role in exist_group.PermissionsKeys)
                    {
                        var rolename = await _roleManager.FindByIdAsync(role.RoleId);
                        if (rolename == null || rolename.Name == null) continue;
                        new_roles.Add(rolename.Name ?? "");
                    }

                    var remove_roles = user_roles.Where(r => !new_roles.Contains(r)).ToList();
                    var add_roles = new_roles.Where(n => !user_roles.Contains(n)).ToList();

                    await _userManager.AddToRolesAsync(user, add_roles);
                    await _userManager.RemoveFromRolesAsync(user, remove_roles);
                }
            }



            _context.CustomerGroups.Update(exist_group);
            _context.SaveChanges();

            return Ok(new Result
            {
                Status = "Success",
                Message = "Customers group succesfully updated",
                Entity = user_group

            });

        }

        /// <summary>
        /// Delete users group by id
        /// </summary>
        /// <param name="group_id">Group Id</param>
        /// <returns>Ok if sucess</returns>
        /// <response code="00">If group is not exist</response>
        /// <response code="500">If role deleting unsuccessfull (have users related)</response>
        [Authorize(Roles = "CustomersWrite,Admin")]
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
