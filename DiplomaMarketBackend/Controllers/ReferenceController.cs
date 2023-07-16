using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Models;
using DiplomaMarketBackend.Helpers;
using DiplomaMarketBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomaMarketBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReferenceController : Controller
    {
        ILogger<ReferenceController> _logger;
        BaseContext _context;

        public ReferenceController(ILogger<ReferenceController> logger, BaseContext context)
        {
            _logger = logger;
            _context = context;

        }
        
        /// <summary>
        /// Order statuses enum values and names
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("order-status")]
        public IActionResult GetOrderStatuses()
        {
            var list = new List<dynamic>();
            
            foreach (OrderStatus status in Enum.GetValues(typeof(OrderStatus)))
            {
                list.Add(new
                {
                    id= status,
                    name = status.ToString()
                });
            }

            return Ok(list);
        }

        /// <summary>
        /// System languages reference list
        /// </summary>
        /// <returns>List of ids:languages</returns>
        [HttpGet]
        [Route("languages")]
        [ResponseCache(Duration = 36000)]
        public async Task<IActionResult> Languages()
        {
            var languages = _context.Languages.ToList();

            return new JsonResult(languages);
        }

        /// <summary>
        /// List of comparability values
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("comparability")]
        [ResponseCache(Duration = 36000)]
        public IActionResult Comparability()
        {

            var compares = new string[] { "main", "disable", "advanced", "bottom" };

            return new JsonResult(new { data = compares });

        }

        /// <summary>
        /// List of areas of Ukraine
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("areas")]
        [ResponseCache(Duration = 36000)]
        public IActionResult GetAreas([FromQuery] string lang)
        {
            lang = lang.NormalizeLang();

            var areas = _context.Areas.
                Include(a => a.Name.Translations).
                ToList();

            var response = new List<dynamic>();

            foreach (var area in areas)
            {

                response.Add(new
                {
                    id = area.Id,
                    name = area.Name != null ? area.Name.Content(lang) : area.Description
                });
            }

            return Json(response);
        }

        /// <summary>
        /// Get cities list of given area id 
        /// </summary>
        /// <param name="area_id">Area id from area list</param>
        /// <param name="lang">language</param>
        /// <returns></returns>
        [HttpGet]
        [Route("cities")]
        [ResponseCache(Duration = 36000)]
        public IActionResult GetAreasCity([FromQuery] int area_id, string lang)
        {
            lang = lang.NormalizeLang();

            var cities = _context.Cities.
                Include(a => a.Name.Translations).
                Where(c => c.AreaId == area_id).
                ToList();

            var response = new List<dynamic>();

            foreach (var city in cities)
            {

                response.Add(new
                {
                    id = city.Id,
                    name = city.Name.Content(lang)
                });
            }

            return Json(response);

        }


        [HttpPost]
        [Route("franchise")]
        public async Task<IActionResult> FranchiseForm([FromForm]string formData, IFormFile [] images )
        {

            _logger.LogInformation(formData);


            if( images == null )
            {
                return new JsonResult(new Result
                {
                    Status = "Error",
                    Message = "Додайте документи!"

                });
            }


            return new JsonResult ( new Result
            {
                Status="Success",
                Message="Форму прийнято!"
            });
        }

    }
}
