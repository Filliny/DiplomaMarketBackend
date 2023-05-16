using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Models;
using DiplomaMarketBackend.Helpers;
using DiplomaMarketBackend.Models;
using Lessons3.Entity.Models;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PasswordGenerator;
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

        public OrdersController(ILogger<WorkController> logger, BaseContext context, IFileService fileService, IEmailService emailService, UserManager<UserModel> userManager)
        {
            _logger = logger;
            _context = context;
            _fileService = fileService;
            _emailService = emailService;
            _userManager = userManager;
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
                    name = pay.Name.Content(lang),
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
                        name = sub.Name.Content(lang),
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

                        if (order.user.name.IsNullOrEmpty() || order.user.email.IsNullOrEmpty())
                            return BadRequest(new Result
                            {
                                Status = "Error",
                                Message = "Some values is null",
                                Entity = order.user
                            });

                        UserModel user = new()
                        {
                            Email = order.user.email,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            UserName = order.user.email,
                            EmailConfirmed = true
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

            //add reciver
            if (order.receiver == null)
                order.receiver = order.user.Adapt<Order.Receiver>();

            var receiver = await _context.Receivers.
                FirstOrDefaultAsync(r => r.Email.Equals(order.receiver.email) && r.UserId == logged_user.Id);

            if (receiver == null)
            {
                receiver = order.receiver.Adapt<ReceiverModel>() ?? new ReceiverModel();
                receiver.UserId = logged_user.Id;
                _context.Receivers.Add(receiver);
            }
            //order process
            if (order.order_summ != await GetOrderSumm(order)) return BadRequest(new Result { Status = "Error", Message = "Order summ calculation is wrong" });

            var new_order = new OrderModel
            {

                User = logged_user,
                CreatedAt = DateTime.Now,
                Status = OrderStatus.New,
                Receiver = receiver,
                PaymentTypeId = order.payData.payment_type_id,
                PaymentData = JsonConvert.SerializeObject(order.payData),
                TotalPrice = order.order_summ,
                DeliveryBranchId = order.delivery_branch_id
            };


            foreach (var item in order.goods)
            {
                var article = await _context.Articles.FindAsync(item.Key);
                if (article == null) return BadRequest(new Result { Status = "Error", Message = $"Article id:{item.Key} not exist!" });

                var orderitem = new OrderItemModel
                {
                    Article = article,
                    Quantity = item.Value
                };

                new_order.Items.Add(orderitem);
            }

            foreach (var cert in order.certificates)
            {
                var certificate = await _context.Certificates.FirstOrDefaultAsync(c => c.CertificateCode.Equals(cert));
                if (certificate == null) return BadRequest(new Result { Status = "Error", Message = $"Certificate id:{cert} not exist!" });
                if (certificate.ValidUntil.Date < DateTime.Now.Date || !certificate.Unused) return BadRequest(new Result { Status = "Error", Message = $"Certificate id:{cert} valid untill {certificate.ValidUntil.ToShortDateString()} is expired or already Used!" });

                certificate.Closed = DateTime.Now;
                certificate.Unused = false;
                _context.Certificates.Update(certificate);
                new_order.Certificates.Add(certificate);
            }

            foreach (var promo in order.promo_codes)
            {
                var promocode = await _context.PromoCodes.FirstOrDefaultAsync(c => c.PromoCode.Equals(promo));
                if (promocode == null) return BadRequest(new Result { Status = "Error", Message = $"Promocode id:{promo} not exist!" });
                if (promocode.ValidUntil.Date < DateTime.Now.Date || promocode.Closed != null) return BadRequest(new Result { Status = "Error", Message = $"Promocode id:{promo} expired!" });
                new_order.PromoCodes.Add(promocode);
            }

            _context.Orders.Add(new_order);
            _context.SaveChanges();

            var payment_type = await _context.PaymentTypes.FindAsync(new_order.PaymentTypeId);

            return Ok(new Result
            {
                Status = "Success",
                Message = "Order creatd Successfully!",
                Entity = new
                {
                    jwt,
                    payment_callback = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + "/api/Order/liqpay-callback", //payment_type.CallbackURL
                    order_id = new_order.Id,
                    order = new_order,
                }
            });
        }

        /// <summary>
        /// Get order by id
        /// </summary>
        /// <param name="order_id">Order id</param>
        /// <returns>Order entity serialized</returns>
        /// <response code="404">If order not found</response>
        [HttpPost]
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
            result.goods = order.Items.ToDictionary(t => t.ArticleId, t => t.Quantity);
            result.order_summ = order.TotalPrice;

            return Ok(result);
        }

        private async Task<decimal> GetOrderSumm(Order order)
        {
            decimal total_summ = default;

            foreach (var item in order.goods)
            {
                var article = await _context.Articles.FindAsync(item.Key);
                if (article == null) continue;

                total_summ += article.Price * item.Value;
            }

            foreach (var cert in order.certificates)
            {
                var certificate = await _context.Certificates.FirstOrDefaultAsync(c => c.CertificateCode.Equals(cert));
                if (certificate == null) continue;
                total_summ -= certificate.Summ;
            }

            foreach (var promo in order.promo_codes)
            {
                var promocode = await _context.PromoCodes.FirstOrDefaultAsync(c => c.PromoCode.Equals(promo));
                if (promocode == null) continue;
                total_summ -= promocode.Summ;
            }

            return total_summ;
        }

    }
}
