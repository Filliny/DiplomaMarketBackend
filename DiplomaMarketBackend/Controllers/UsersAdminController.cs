﻿using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Models;
using DiplomaMarketBackend.Entity.Models;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using WebShopApp.Abstract;

namespace DiplomaMarketBackend.Controllers
{

    //[Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersAdminController : Controller
    {
        private readonly ILogger<UsersAdminController> _logger;
        private readonly BaseContext _context;
        private readonly IEmailService _emailService;
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public UsersAdminController(BaseContext context, ILogger<UsersAdminController> logger, IEmailService emailService, UserManager<UserModel> userManager,
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
        /// Get all users list /or filtered by users group id
        /// </summary>
        /// <param name="group">Users group id</param>
        /// <returns>List of users</returns>
        [HttpGet]
        [Route("get-users")]
        public async Task<IActionResult> Get([FromQuery] int? group)
        {

            IList<UserModel>? users = null;

            if (group != null)
                users = await _context.Users.Include(u => u.CustomerGroup).
                    Where(u => u.CustomerGroupId == group).ToListAsync();
            else
                users = await _context.Users.Include(u => u.CustomerGroup).
                    Where(u => u.CustomerGroupId != null).ToListAsync();

            var result = new List<dynamic>();
            foreach (var user in users)
            {
                result.Add(new
                {
                    id = user.Id,
                    first_name = user.FirstName,
                    middle_name = user.MiddleName,
                    last_name = user.LastName,
                    preferred_language = user.PreferredLanguage,
                    register_date = user.RegDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                    birth_date = user.BirthDay.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                    email = user.Email,
                    phone = user.PhoneNumber,
                    group = user.CustomerGroup?.Name ?? "",
                    group_id = user.CustomerGroup?.Id,
                });
            }

            return Json(result);

        }


        /// <summary>
        /// Get user by email for editing
        /// </summary>
        /// <param name="email">User Id</param>
        /// <returns>UserFull entity</returns>
        /// <response code="400">If the request values is bad</response>
        [HttpGet]
        [Route("get-email")]
        public async Task<ActionResult<UserFull>> GetUserByEmail([FromQuery] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);


            if (user != null)
            {
                var user_dto = user.Adapt<UserFull>();
                var role = await _userManager.GetRolesAsync(user);
                user_dto.roles = role;
                return Json(user_dto);
            }

            return BadRequest(new Result
            {
                Status = "Error",
                Message = "User not found"
            });

        }

        /// <summary>
        /// Get user by id for editing
        /// </summary>
        /// <param name="user_id">User Id</param>
        /// <returns>UserFull entity</returns>
        /// <response code="400">If the request values is bad</response>
        [HttpGet]
        [Route("get")]
        public async Task<ActionResult<UserFull>> GetUser([FromQuery] string user_id)
        {
            var user = await _userManager.FindByIdAsync(user_id);


            if (user != null)
            {
                var user_dto = user.Adapt<UserFull>();
                user_dto.roles = await _userManager.GetRolesAsync(user);
                return Json(user_dto);
            }

            return BadRequest(new Result
            {
                Status = "Error",
                Message = "User not found"
            });

        }


        /// <summary>
        /// Create new user 
        /// </summary>
        /// <param name="user">User entity</param>
        /// <returns>Ok with UserFull Entity with new id</returns>
        /// <response code="400">If the request values is bad</response>
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<UserFull>> CreateUser([FromBody] UserFull user)
        {
            try
            {
                user.user_name = user.email;
                var mapsterConfig = TypeAdapterConfig.GlobalSettings.Clone();
                mapsterConfig.Default.Ignore("Id").Ignore("id");

                var usr = CreateUser();
                var new_user = user.Adapt<UserModel>(mapsterConfig);

                new_user.RegDate = DateTime.Now;

                if (user.password == null) throw new Exception("Password is null!");

                var result = await _userManager.CreateAsync(new_user, user.password);

                if (result.Succeeded)
                {

                    var roles_result = await ChangeUserGroup(new_user, null, user.customer_group_id);

                    if (roles_result)
                    {
                        if (user.email_notify && new_user.Email != null)
                        {
                            await _emailService.SendEmailAsync(new_user.Email, "Account created",
                                 $"<h5>Admin just created your account with login:{user.email} and password: {user.password}");
                        }

                        user.roles = await _userManager.GetRolesAsync(new_user);
                        user.Id = new_user.Id;

                        return Ok(new Result
                        {
                            Status = "Success",
                            Message = "User created sucessfully",
                            Entity = user

                        });

                    }
                    else
                    {
                        return BadRequest(new Result
                        {
                            Status = "Error",
                            Message = "Create user error- Permission assign error",
                            Entity = new
                            {
                                user.customer_group_id,
                            }
                        });

                    }

                }
                else
                {
                    return BadRequest(new Result
                    {
                        Status = "Error",
                        Message = "Create user error- see entity",
                        Entity = result.Errors
                    });

                }

            }
            catch (Exception ex)
            {

                return BadRequest(new Result
                {
                    Status = "Error",
                    Message = "Create user error: " + ex.Message
                });
            }

        }

        /// <summary>
        /// Update user data and roles
        /// </summary>
        /// <param name="user">User entity for update</param>
        /// <returns>Ok if success</returns>
        /// <response code="400">If the request values is bad</response>
        [HttpPut]
        [Route("update")]
        public async Task<ActionResult<UserFull>> UpdateUser([FromBody] UserFull user)
        {
            try
            {
                var exist_user = await _userManager.FindByIdAsync(user.Id);
                if (exist_user == null) throw new Exception("User not found!");

                user.user_name = user.email;
                var oldgroup = exist_user.CustomerGroupId;
                user.Adapt(exist_user);


                var result = await _userManager.UpdateAsync(exist_user);

                if (result.Succeeded)
                {

                    if (await ChangeUserGroup(exist_user, oldgroup, user.customer_group_id))//roles_result.Succeeded)
                    {
                        user.roles = await _userManager.GetRolesAsync(exist_user);
                        return Ok(new Result
                        {
                            Status = "Success",
                            Message = "User updated sucessfully",
                            Entity = user

                        });

                    }
                    else
                    {
                        return BadRequest(new Result
                        {
                            Status = "Error",
                            Message = "Update user error - see entity",
                            Entity = null /// roles_result.Errors
                        });

                    }

                }
                else
                {
                    return BadRequest(new Result
                    {
                        Status = "Error",
                        Message = "Update user error- see entity",
                        Entity = result.Errors
                    });

                }

            }
            catch (Exception ex)
            {

                return BadRequest(new Result
                {
                    Status = "Error",
                    Message = "Create user error: " + ex.Message
                });
            }

        }


        /// <summary>
        /// Lock user by Id
        /// </summary>
        /// <param name="user_id">User Id</param>
        /// <returns>Ok if sucess</returns>
        /// <response code="500">If fail remove User</response>
        [HttpPatch]
        [Route("lock")]
        public async Task<ActionResult<UserFull>> LockUser([FromQuery] string user_id)
        {
            try
            {
                var exist_user = await _userManager.FindByIdAsync(user_id);
                if (exist_user == null) throw new Exception("User not found!");

                var result = await _userManager.SetLockoutEnabledAsync(exist_user, true);

                if (result.Succeeded)
                {
                    return Ok(new Result
                    {
                        Status = "Success",
                        Message = "User locked",

                    });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Result
                    {
                        Status = "Error",
                        Message = "User not locked!",
                        Entity = result.Errors
                    });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Result
                {
                    Status = "Error",
                    Message = "User not locked!",
                    Entity = ex.Message
                });
            }
        }


        /// <summary>
        /// Unock user by Id
        /// </summary>
        /// <param name="user_id">User Id</param>
        /// <returns>Ok if sucess</returns>
        /// <response code="500">If fail remove User</response>
        [HttpPatch]
        [Route("unlock")]
        public async Task<ActionResult<UserFull>> UnockUser([FromQuery] string user_id)
        {
            try
            {
                var exist_user = await _userManager.FindByIdAsync(user_id);
                if (exist_user == null) throw new Exception("User not found!");

                var result = await _userManager.SetLockoutEnabledAsync(exist_user, false);

                if (result.Succeeded)
                {
                    return Ok(new Result
                    {
                        Status = "Success",
                        Message = "User unlocked",

                    });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Result
                    {
                        Status = "Error",
                        Message = "User not unocked!",
                        Entity = result.Errors
                    });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Result
                {
                    Status = "Error",
                    Message = "User not unlocked!",
                    Entity = ex.Message
                });
            }
        }


        /// <summary>
        /// Delete user by Id
        /// </summary>
        /// <param name="user_id">User Id</param>
        /// <returns>Ok if sucess</returns>
        /// <response code="500">If fail remove User</response>
        [HttpDelete]
        [Route("delete")]
        public async Task<ActionResult<UserFull>> RemoveUser([FromQuery] string user_id)
        {
            try
            {
                var exist_user = await _userManager.FindByIdAsync(user_id);
                if (exist_user == null) throw new Exception("User not found!");

                var result = await _userManager.DeleteAsync(exist_user);

                if (result.Succeeded)
                {
                    return Ok(new Result
                    {
                        Status = "Success",
                        Message = "User removed",

                    });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Result
                    {
                        Status = "Error",
                        Message = "User not removed!",
                        Entity = result.Errors
                    });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Result
                {
                    Status = "Error",
                    Message = "User not removed!",
                    Entity = ex.Message
                });
            }
        }


        private UserModel CreateUser()
        {
            try
            {
                return Activator.CreateInstance<UserModel>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(UserModel)}'. " +
                    $"Ensure that '{nameof(UserModel)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private async Task<bool> ChangeUserGroup(UserModel user, int? oldGroupId, int? newGroupId)
        {
            //check values
            if (user == null) return false;
          
            if (oldGroupId == 0 || newGroupId == 0) return false;
            if (oldGroupId == newGroupId) return true;

            //remove all old roles
            if (oldGroupId != null)
            {
                var oldGroup = await _context.CustomerGroups.Include(g => g.PermissionsKeys).FirstOrDefaultAsync(g => g.Id == oldGroupId);
                if (oldGroup != null)
                    foreach (var key in oldGroup.PermissionsKeys)
                    {
                        var role = await _roleManager.FindByIdAsync(key.RoleId);
                        if (role != null && role.Name != null)
                            await _userManager.RemoveFromRoleAsync(user, role.Name);
                    }
            }
            else
            {   //if any simple roles present
                var roles = await _userManager.GetRolesAsync(user);
                await  _userManager.RemoveFromRolesAsync(user, roles);
            }

            //add to new roles
            if (newGroupId != null)
            {
                var newGroup = await _context.CustomerGroups.Include(g => g.PermissionsKeys).FirstOrDefaultAsync(g => g.Id == newGroupId);
                if (newGroup != null)
                    foreach (var key in newGroup.PermissionsKeys)
                    {
                        var role = await _roleManager.FindByIdAsync(key.RoleId);
                        if (role != null && role.Name != null)
                            await _userManager.AddToRoleAsync(user, role.Name);
                    }
            }
            else//if NULL assign User role
            {
                user.CustomerGroupId = null;
                await _userManager.AddToRoleAsync(user,"Admin");
            }

            return true;
        }
    }
}
