using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Models;
using DiplomaMarketBackend.Models;
using Lessons3.Entity.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Text;
using System.Text.Encodings.Web;
using WebShopApp.Abstract;
using static DiplomaMarketBackend.Models.Order;

namespace DiplomaMarketBackend.Controllers
{
    /// <summary>
    /// Authorize required!
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersCabinetController : Controller
    {
        private readonly ILogger<UsersAdminController> _logger;
        private readonly BaseContext _context;
        private readonly IEmailService _emailService;
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public UsersCabinetController(BaseContext context, ILogger<UsersAdminController> logger, IEmailService emailService, UserManager<UserModel> userManager,
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
        /// Get current user data for editing
        /// </summary>
        /// <returns>UserFull entity</returns>
        /// <response code="400">If the request values is bad</response>
        [HttpGet]
        [Route("get")]
        public async Task<ActionResult<UserFull>> GetUser()
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name ?? "");

            if (user != null)
            {
                var user_dto = user.Adapt<UserFull>();
                var role = await _userManager.GetRolesAsync(user);

                //if (role != null)
                //{
                //    user_dto.roles = role;
                //}

                return Json(user_dto);
            }

            return BadRequest(new Result
            {
                Status = "Error",
                Message = "User not found"
            });
        }


        /// <summary>
        /// Update user personal data in cabinet
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
                var exist_user = await _userManager.FindByNameAsync(User.Identity?.Name ?? "");
                if (exist_user == null) throw new Exception("User not found! Current User = "+ User.Identity?.Name??"");

                //no mapping cos we can update wrong data
                exist_user.FirstName = user.first_name;
                exist_user.LastName = user.last_name;
                exist_user.MiddleName = user.middle_name;
                exist_user.PreferredLanguageId = user.preferred_language_id;
                exist_user.BirthDay = user.birth_day;

                var result = await _userManager.UpdateAsync(exist_user);

                if (result.Succeeded)
                {
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
                    Message = "Update user error: " + ex.Message
                });
            }

        }

        /// <summary>
        /// Get list of receivers
        /// </summary>
        /// <returns>List or user receivers</returns>
        [HttpGet]
        [Route("get-receivers")]
        public async Task<IActionResult> GetReceivers()
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name ?? "");

            if (user != null)
            {
                var receivers = await _context.Receivers.Where(r => r.UserId == user.Id).ToListAsync();

                var receivers_dto = receivers.Adapt<List<Order.Receiver>>();


                return Json(receivers_dto);
            }

            return BadRequest(new Result
            {
                Status = "Error",
                Message = "User not found"
            });

        }

        /// <summary>
        /// Create new receiver
        /// </summary>
        /// <param name="receiver"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create-receiver")]
        public async Task<ActionResult<Order.Receiver>> CreateReceiver([FromForm] Receiver receiver)
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name ?? "");

            if (user != null)
            {
                var new_receiver = receiver.Adapt<ReceiverModel>();
                new_receiver.UserId = user.Id;

                _context.Receivers.Add(new_receiver);
                _context.SaveChanges();
                receiver.Id = new_receiver.Id;
                return Ok(
                    new Result
                    {
                        Status = "Success",
                        Message = "Receiver created",
                        Entity = receiver
                    });
            }

            return BadRequest(new Result
            {
                Status = "Error",
                Message = "Receiver not found"
            });

        }

        /// <summary>
        /// Update receiver data
        /// </summary>
        /// <param name="receiver">Data from form</param>
        /// <returns>Ok if success</returns>
        [HttpPut]
        [Route("update-receiver")]
        public async Task<ActionResult<Order.Receiver>> UpdateReceiver([FromForm] Receiver receiver)
        {
            var ex_receiver = await _context.Receivers.FindAsync(receiver.Id);
            var user = await _userManager.FindByNameAsync(User.Identity?.Name ?? "");

            if (ex_receiver != null && ex_receiver.UserId == user.Id)
            {
                receiver.Adapt(ex_receiver);
                _context.SaveChanges();

                return Ok(new Result
                {
                    Status = "Success",
                    Message = "Receiver updated",
                    Entity = receiver
                });
            }

            return BadRequest(new Result
            {
                Status = "Error",
                Message = "Receiver not found"
            });

        }

        /// <summary>
        /// Delete receiver by id
        /// </summary>
        /// <param name="id">Id of receiver</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("delete-receiver")]
        public async Task<IActionResult> DeleteReceiver([FromQuery] int id)
        {
            var ex_receiver = await _context.Receivers.FindAsync(id);
            var user = await _userManager.FindByNameAsync(User.Identity?.Name ?? "");

            if (ex_receiver != null && ex_receiver.UserId == user.Id)
            {
                _context.Receivers.Remove(ex_receiver);
                _context.SaveChanges();

                return Ok(new Result
                {
                    Status = "Success",
                    Message = "Receiver deleted"
                });
            }

            return BadRequest(new Result
            {
                Status = "Error",
                Message = "Receiver not found"
            });
        }

        /// <summary>
        /// Get current user delivery address
        /// </summary>
        /// <returns>User delivery adress</returns>
        [HttpGet]
        [Route("address")]
        public async Task<ActionResult<DeliveryAdress>> GetDeliveryAddress()
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name ?? "");

            if (user != null)
            {
                var delivery_address = JsonConvert.DeserializeObject<DeliveryAdress>(user.DeliveryAddress) ?? new DeliveryAdress();
                return Json(delivery_address);
            }

            return BadRequest(new Result
            {
                Status = "Error",
                Message = "User not found"
            });
        }

        /// <summary>
        /// Update user delivery address
        /// </summary>
        /// <param name="address">Delivery adress entity </param>
        /// <returns></returns>
        [HttpPut]
        [Route("address-update")]
        public async Task<ActionResult<DeliveryAdress>> SetDeliveryAddress([FromForm] DeliveryAdress address)
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name ?? "");

            if (user != null)
            {
                user.DeliveryAddress = JsonConvert.SerializeObject(address);

                await _userManager.UpdateAsync(user);
                return Ok(address);
            }

            return BadRequest(new Result
            {
                Status = "Error",
                Message = "User not found"
            });
        }

        /// <summary>
        /// Update user contacts
        /// </summary>
        /// <param name="contacts"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("contacts-update")]
        public async Task<ActionResult<UserContacts>> SetContacts([FromForm] UserContacts contacts)
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name ?? "");

            if (user != null)
            {
                user.PhoneNumber = contacts.phone;
                user.Email = contacts.email;

                await _userManager.UpdateAsync(user);
                return Ok(contacts);
            }

            return BadRequest(new Result
            {
                Status = "Error",
                Message = "User not found"
            });
        }

        /// <summary>
        /// Change login email (user name) for user cabinet; 
        /// requires confirmation by email
        /// </summary>
        /// <param name="contacts">Contacts entity with new email</param>
        /// <param name="front_callback">Frontend callback for username change confirm route e.g. http://your_host/confirm_email</param>
        /// <returns>Returns ok if success</returns>
        [HttpPost]
        [Route("login-change")]
        public async Task<IActionResult> ChangeLogin([FromForm] UserContacts contacts, [FromQuery] string front_callback)
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name ?? "");
            var old_email = await _userManager.GetEmailAsync(user);

            if (user != null && old_email != null)
            {

                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, contacts.email);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = $"{front_callback}?email={contacts.email}&code={code}";

                await _emailService.SendEmailAsync(
                    contacts.email,
                    "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                if (user.Email != null)
                    await _emailService.SendEmailAsync(
                       user.Email,
                       "We register attemt to change your login username",
                       $"If there wasn't You - please change your password and/or refer to support");

                return Ok(new Result
                {
                    Status = "Success",
                    Message = "Confirmation link to change email sent. Please check your email.",
                    Entity = contacts,
                });

            }

            return BadRequest(new Result
            {
                Status = "Error",
                Message = "Change user name error",
                Entity = contacts
            });

        }

        /// <summary>
        /// Login change  confirm endpoint for user cabinet 
        /// </summary>
        /// <param name="new_email">New email form data from callback frontend page</param>
        /// <param name="email_code">Email code form data from callback frontend page </param>
        /// <returns></returns>
        [HttpPost]
        [Route("login-change-confirm")]
        public async Task<IActionResult> ChangeLoginConfirm([FromForm] string new_email, [FromForm] string email_code)
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name ?? "");

            if (user != null)
            {
                var old_email = user.Email;
                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(email_code));

                //to dispose email code 
                var result = await _userManager.ChangeEmailAsync(user, old_email ?? new_email, code);
                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Result
                    {
                        Status = "Error",
                        Message = "Error changing email.",
                        Entity = new_email
                    });

                }
                else
                {
                    var setUserNameResult = await _userManager.SetUserNameAsync(user, new_email);
                    if (!setUserNameResult.Succeeded)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new Result
                        {
                            Status = "Error",
                            Message = "Error changing user name.",
                            Entity = new_email
                        });

                    }

                    return Ok(new Result
                    {
                        Status = "Success",
                        Message = "Sucessfully changing user name.",
                        Entity = new_email
                    });
                }

            }

            return StatusCode(StatusCodes.Status500InternalServerError, new Result
            {
                Status = "Error",
                Message = "Error changing user name.",
                Entity = new_email
            });
        }
    }
}
