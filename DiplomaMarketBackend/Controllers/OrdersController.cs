using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Models;
using DiplomaMarketBackend.Helpers;
using DiplomaMarketBackend.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PasswordGenerator;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using MongoDB.Driver.Linq;
using WebShopApp.Abstract;


namespace DiplomaMarketBackend.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : Controller
    {
        ILogger<WorkController> _logger;
        BaseContext _context;
        IFileService _fileService;
        private readonly IEmailService _emailService;
        private readonly UserManager<UserModel> _userManager;
        private readonly IConfiguration _configuration;
        IOptions<LiqpaySettings> _liqpayOptions;

        public OrdersController(ILogger<WorkController> logger, BaseContext context, IFileService fileService, IEmailService emailService, UserManager<UserModel> userManager, IConfiguration configuration, IOptions<LiqpaySettings> liqpayOptions)
        {
            _logger = logger;
            _context = context;
            _fileService = fileService;
            _emailService = emailService;
            _userManager = userManager;
            _configuration = configuration;
            _liqpayOptions = liqpayOptions;
        }
        
        
        /// <summary>
        /// Orders list for admin page - for new and in process list
        /// Нові замовлення, Замовлення в обробці (only_new = false)
        /// </summary>
        /// <param name="lang">Language</param>
        /// <param name="page">Display page</param>
        /// <param name="limit">Items on page</param>
        /// <param name="only_new">Only new orders show</param>
        /// <returns>List of orders</returns>
        [HttpGet]
        [Route("orders-list")]
        //[Authorize(Roles = "OrdersRead")]
        public async Task<IActionResult> GetOrdersList([FromQuery] string lang, int page, int limit=10, bool only_new = true)
        {
            var ordersq = _context.Orders.
                Include(o=>o.User).
                Where(o => o.Status != OrderStatus.Closed);

            if (only_new)
            {
                ordersq = ordersq.Where(o => o.Status == OrderStatus.New);
            }
            else
            {
                ordersq = ordersq.Where(o => o.Status != OrderStatus.New);
            }
            
            var orders = await ordersq.ToListAsync();
            

            int totalOrders = orders.Count;
            int totalPages = (int)Math.Ceiling((decimal)totalOrders / (decimal)limit);
            
            if (page > totalPages) page = totalPages;
            int skip = (page - 1) * limit;
            
            orders = orders.Skip(skip).Take(limit).ToList();

            var outlist = new List<dynamic>();
            foreach (var order in orders)
            {
                var userOrders = await _context.Orders.OrderBy(o => o.CreatedAt).
                    Where(o => o.UserId == order.UserId)
                    .ToListAsync();
                
                outlist.Add(new
                {
                    id = order.Id,
                    payment_status = "payed",
                    orders = userOrders.Count.ToString()+" від "+userOrders.First().CreatedAt.ToString("dd MM yyyy"),
                    buyer_name = order.User?.LastName??""+" "+ order.User?.FirstName??"",
                    phone = order.User?.PhoneNumber,
                    order_date = order.CreatedAt.ToString("dd MM yyyy"),
                    status = order.Status
                    
                });
            }
            
            var result = new
            {
                data = new
                {
                    orders = outlist,
                    total_goods = totalOrders,
                    total_pages = totalPages,
                    displayed_page = page
                }
            };

            return Ok(result);
        }

        /// <summary>
        /// Check certificate id - use 11110000 for test 100UAH
        /// </summary>
        /// <param name="certificate_code"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("certificate-check")]
        public async Task<IActionResult> CheckSertificate([FromQuery] string certificate_code)
        {
            if (certificate_code.Equals("11110000"))
            {

                var result = new
                {
                    id = 0,
                    state = "valid",
                    is_active = true,
                    summ = 100.00
                };

                return Ok(result);
            }
            else
            {
                var cert = await _context.Certificates.FirstOrDefaultAsync(c => c.CertificateCode.Equals(certificate_code));

                if (cert != null)
                {
                    var result = new
                    {
                        id = cert.Id,
                        state = "valid",
                        is_active = cert.Unused && cert.ValidUntil > DateTime.Now,
                        summ = cert.Summ
                    };

                    return Ok(result);
                }
                else return BadRequest(new Result
                {
                    Status = "Error",
                    Message = "Certificate not found!",
                });
            }
        }

        /// <summary>
        /// Check promocode - use LADNAHATA for test -100UAH
        /// </summary>
        /// <param name="promo_code"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("promo-check")]
        public async Task<IActionResult> CheckPromocode([FromQuery] string promo_code)
        {
            if (promo_code.Equals("LADNAHATA"))
            {

                var result = new
                {
                    id = 0,
                    state = "valid",
                    is_active = true,
                    summ = 100.00
                };

                return Ok(result);
            }
            else
            {
                var promo = await _context.PromoCodes.FirstOrDefaultAsync(c => c.PromoCode.Equals(promo_code));

                if (promo != null)
                {
                    var result = new
                    {
                        id = promo.Id,
                        state = "valid",
                        is_active = promo.ValidUntil > DateTime.Now,
                        summ = promo.Summ
                    };

                    return Ok(result);
                }
                else return BadRequest(new Result
                {
                    Status = "Error",
                    Message = "Promocode not found!",

                });
            }
        }

        /// <summary>
        /// Get payment methods with ids for payment order form
        /// </summary>
        /// <param name="lang">language</param>
        /// <returns></returns>
        [HttpGet]
        [Route("payment-methods")]
        [ResponseCache(VaryByQueryKeys = new[] { "lang" }, Duration = 3600)]
        public async Task<IActionResult> GetAllPayments([FromQuery] string lang)
        {
            lang = lang.NormalizeLang();

            var payments = await _context.PaymentTypes.
                Include(p => p.Name.Translations).
                Include(p => p.Description.Translations).
                Include(p => p.Childs).
                ToListAsync();

            var result = new List<dynamic>();
            foreach (var pay in payments)
            {
                if (pay.Parent != null) continue;

                var head = new
                {
                    id = pay.Id,
                    name = pay.Name?.Content(lang),
                    description = pay.Description == null ? "" : pay.Description.Content(lang),
                    callback = pay.CallbackURL,
                    sub_methods = new List<dynamic>()
                };

                pay.Childs = pay.Childs.OrderBy(pay => pay.Id).ToList();

                foreach (var sub in pay.Childs)
                {
                    head.sub_methods.Add(new
                    {
                        id = sub.Id,
                        name = sub.Name?.Content(lang),
                        description = sub.Description == null ? "" : sub.Description.Content(lang),
                        callback = sub.CallbackURL,

                    });
                }

                result.Add(head);
            }

            return Ok(result);
        }

        /// <summary>
        /// Create new order
        /// </summary>
        /// <param name="order">Order entity from body Json</param>
        /// <returns></returns>
        /// <response code="409">If user with email given exist but not logged - need redirect to login page</response>
        /// <response code="400">If some values is bad</response>
        /// <response code="500">User or order creation error</response>
        [HttpPost]
        [Route("new")]
        public async Task<ActionResult<Order>> NewOrder([FromBody] Order order)
        {
            var logged_user = await _userManager.FindByNameAsync(User.Identity.Name ?? "");

            //check user not logged
            if (logged_user == null)
            {
                //check used data filled
                if (order.user != null)
                {
                    var exist_user = await _userManager.FindByEmailAsync(order.user.email);

                    if (exist_user != null)
                    {
                        return Conflict(new Result
                        {
                            Status = "Error",
                            Message = "User with email exist - proceed to login !",
                            Entity = order.user
                        });

                    }
                    else//register new user
                    {

                        if (order.user.first_name.IsNullOrEmpty() || order.user.email.IsNullOrEmpty())
                            return BadRequest(new Result
                            {
                                Status = "Error",
                                Message = "Some values is null",
                                Entity = order.user
                            });

                        UserModel user = new()
                        {
                            FirstName = order.user.first_name,
                            LastName = order.user.last_name,
                            Email = order.user.email,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            UserName = order.user.email,
                            EmailConfirmed = true,
                            RegDate = DateTime.Now,

                        };

                        var pwd = new Password().IncludeLowercase().IncludeUppercase().IncludeNumeric().IncludeSpecial("[]{}^_=");
                        var password = pwd.Next();

                        var result = await _userManager.CreateAsync(user, password);
                        if (result.Succeeded)
                        {
                            var role_result = await _userManager.AddToRoleAsync(user, "User");
                            if (!role_result.Succeeded)
                            {
                                return StatusCode(StatusCodes.Status500InternalServerError, new Result
                                {
                                    Status = "Error",
                                    Message = "User creation failed! Please check user details and try again.",
                                    Entity = role_result.Errors
                                });
                            }
                            logged_user = user;
                            await _emailService.SendEmailAsync(logged_user.Email, "New registration", HtmlEncoder.Default.Encode($"<h1>Password :{password}</h1>"));
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError, new Result
                            {
                                Status = "Error",
                                Message = "User creation failed! Please check user details and try again.",
                                Entity = result.Errors
                            });
                        }

                    }
                }
                else
                {
                    return BadRequest(new Result { Status = "Error", Message = "User creation failed! Specify user data for order!" });
                }

            }

            var jwt = JwtTokenGenerator.GetToken(_userManager, logged_user);

            //process delivery adress
            if (order.delivery_adress != null && logged_user.DeliveryAddress.IsNullOrEmpty())
            {
                logged_user.DeliveryAddress = JsonConvert.SerializeObject(order.delivery_adress);
            }

            //add reciver
            if (order.receiver == null)
                order.receiver = order.user.Adapt<Order.Receiver>();

            var receiver = await _context.Receivers.
                FirstOrDefaultAsync(r => r.Email.Equals(order.receiver.email) && r.UserId == logged_user.Id);

            if (receiver == null)
            {
                //fill profile name cos no field if order form
                if (order.receiver.profile_name == null) order.receiver.profile_name = order.receiver.first_name + " " + order.receiver.last_name;
                receiver = order.receiver.Adapt<ReceiverModel>() ?? new ReceiverModel();

                receiver.UserId = logged_user.Id;
                _context.Receivers.Add(receiver);
            }
            //order process
            if (order.total_price != await GetOrderSumm(order)) return BadRequest(new Result { Status = "Error", Message = "Order summ calculation is wrong" });

            var new_order = new OrderModel
            {

                User = logged_user,
                CreatedAt = DateTime.Now,
                Status = OrderStatus.New,
                Receiver = receiver,
                PaymentTypeId = order.payData.payment_type_id,
                PaymentData = JsonConvert.SerializeObject(order.payData),
                TotalPrice = order.total_price,
                DeliveryBranchId = order.delivery_branch_id
            };

            new_order.Items = order.goods.Adapt<List<OrderItemModel>>();
            
            if(order.certificates is not null)
                foreach (var cert in order.certificates)
                {
                    var certificate = await _context.Certificates.FirstOrDefaultAsync(c => c.CertificateCode.Equals(cert));
                    if (certificate == null) return BadRequest(new Result { Status = "Error", Message = $"Certificate id:{cert} not exist!" });
                    if (certificate.ValidUntil.Date < DateTime.Now.Date || !certificate.Unused) return BadRequest(new Result { Status = "Error", Message = $"Certificate id:{cert} valid untill {certificate.ValidUntil.ToShortDateString()} is expired or already Used!" });

                    if ( certificate.CertificateCode != "11110000")//todo clean certificate
                    {
                        certificate.Closed = DateTime.Now;
                        certificate.Unused = false;
                        _context.Certificates.Update(certificate);
                        new_order.Certificates.Add(certificate);
                    }

                }
            
            if(order.promo_codes is not null)
                foreach (var promo in order.promo_codes)
                {
                    var promocode = await _context.PromoCodes.FirstOrDefaultAsync(c => c.PromoCode.Equals(promo));
                    if (promocode == null) return BadRequest(new Result { Status = "Error", Message = $"Promocode id:{promo} not exist!" });
                    if (promocode.ValidUntil.Date < DateTime.Now.Date || promocode.Closed != null) return BadRequest(new Result { Status = "Error", Message = $"Promocode id:{promo} expired!" });
                    new_order.PromoCodes.Add(promocode);
                }

            _context.Orders.Add(new_order);
            _context.SaveChanges();

            //var payment_type = await _context.PaymentTypes.FindAsync(new_order.PaymentTypeId);

            var user_dto = logged_user.Adapt<UserFull>();
            var role = await _userManager.GetRolesAsync(logged_user);

            if (role != null)
            {
                user_dto.roles = role;
            }

            return Ok(new Result
            {
                Status = "Success",
                Message = "Order created Successfully!",
                Entity = new
                {
                    jwt,
                    payment_callback = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + "/api/Order/liqpay-callback", //payment_type.CallbackURL
                    user = user_dto,
                    order_id = new_order.Id,

                }
            });
        }

        /// <summary>
        /// Get order by id
        /// </summary>
        /// <param name="order_id">Order id</param>
        /// <returns>Order entity serialized</returns>
        /// <response code="404">If order not found</response>
        [HttpGet]
        [Route("get")]
        public async Task<ActionResult<Order>> GetOrder([FromQuery] int order_id)
        {
            var order = await _context.Orders.
                Include(o => o.User).
                Include(o => o.Receiver).
                Include(o => o.Items).
                FirstOrDefaultAsync(o => o.Id == order_id);

            if (order == null) return NotFound(new Result
            {
                Status = "Error",
                Message = "Order not found!"
            });

            var result = order.Adapt<Order>();

            result.payData = JsonConvert.DeserializeObject<Order.PayData>(order.PaymentData);
            result.goods = order.Items.Adapt<List<Order.Item>>();


            return Ok(result);
        }

        /// <summary>
        /// Order editing by manager role
        /// </summary>
        /// <param name="order">order data</param>
        /// <returns>Ok if success</returns>
        /// <response code="404">If order not found</response>
        //[Authorize(Roles = "Manager")]
        [HttpPut]
        [Route("update")]
        public async Task<ActionResult<Order>> UpdateOrder([FromBody] Order order)
        {
            var exist_order = await _context.Orders.
                Include(o => o.User).
                Include(o => o.Receiver).
                Include(o => o.Items).
                FirstOrDefaultAsync(o => o.Id == order.id);

            if (order == null) return NotFound(new Result
            {
                Status = "Error",
                Message = "Order not found!"
            });


            //todo define changing values
            //order.Adapt(exist_order);

            return Ok(new Result
            {
                Status = "Sucess",
                Message = "Order updated!",
                Entity = order

            });
        }

        /// <summary>
        /// Order deleting by manager
        /// </summary>
        /// <param name="order_id">order id</param>
        /// <returns>Ok if success</returns>
        /// <response code="400">If order cant be deleted (allready payed and/or articles present)</response>
        /// <response code="404">If order not found</response>
        [Authorize(Roles = "Manager")]
        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteOrder([FromQuery] int order_id)
        {
            var order = await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == order_id);

            if (order == null) return NotFound(new Result
            {
                Status = "Error",
                Message = "Order not found!"
            });

            if (order.Items.Count != 0 && order.Status > OrderStatus.PaymentPending) return BadRequest(
                new Result
                {
                    Status = "Error",
                    Message = "Order can't be deleted!",
                    Entity = order
                });


            return Ok(new Result
            {
                Status = "Sucess",
                Message = "Order updated!",
                Entity = order
            });
        }


        /// <summary>
        /// Cancel order by user
        /// </summary>
        /// <param name="order_id">Order id</param>
        /// <param name="reason">Reason of cancelling from user</param>
        /// <returns>Ok if succesfully cancelled</returns>
        [Authorize]
        [HttpPost]
        [Route("cancel")]
        public async Task<IActionResult> CancelOrder([FromQuery] int order_id, string reason)
        {
            var order = await _context.Orders.FindAsync(order_id);
            if (order == null) return NotFound(new Result
            {
                Status = "Error",
                Message = "Order not found!"
            });

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user != null && user.Id != order.UserId) return Unauthorized(new Result
            {
                Status = "Error",
                Message = "Order not belong to this user!"
            });

            if (order.Status > OrderStatus.InProcess) return BadRequest(new Result
            {
                Status = "Error",
                Message = "Cant cancel shipped order!"
            });


            order.Status = OrderStatus.Cancelled;
            _context.SaveChanges();

            return Ok(new Result
            {
                Status = "Sucess",
                Message = "Order cancelled!"
            });
        }


        /// <summary>
        /// Order summ check method
        /// </summary>
        /// <param name="order">order entity</param>
        /// <returns>order summ</returns>
        private async Task<decimal> GetOrderSumm(Order order)
        {
            decimal total_summ = default;

            foreach (var item in order.goods)
            {
                var article = await _context.Articles.FindAsync(item.article_id);
                if (article == null) continue;

                total_summ += article.Price * item.quantity;
            }

            if(order.certificates is not null)
                foreach (var cert in order.certificates)
                {
                    var certificate = await _context.Certificates.FirstOrDefaultAsync(c => c.CertificateCode.Equals(cert));
                    if (certificate == null) continue;
                    total_summ -= certificate.Summ;
                }
            
            if(order.promo_codes is not null)
                foreach (var promo in order.promo_codes)
                {
                    var promocode = await _context.PromoCodes.FirstOrDefaultAsync(c => c.PromoCode.Equals(promo));
                    if (promocode == null) continue;
                    total_summ -= promocode.Summ;
                }

            return total_summ;
        }


        /// <summary>
        /// Bank callback route
        /// </summary>
        /// <param name="data"></param>
        /// <param name="signature"></param>
        /// <returns>Ok if payment processed</returns>
        [HttpPost]
        [Route("liqpay-callback")]
        public async Task<ActionResult<Order>> LiqpayCallback([FromQuery] string data, string signature)
        {
            var priv_key = _liqpayOptions.Value.PrivateKey;
            var concatenated = priv_key + data + priv_key;
            byte[] sourceBytes = Encoding.UTF8.GetBytes(concatenated);

            var sha1 = SHA1.HashData(sourceBytes);

            var comparable = Convert.ToBase64String(sha1);

            return Ok();
        }
    }
}
