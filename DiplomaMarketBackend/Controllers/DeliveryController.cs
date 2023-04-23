using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomaMarketBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryController : Controller
    {
        ILogger<WorkController> _logger;
        BaseContext _context;
        IFileService _fileService;

        public DeliveryController(ILogger<WorkController> logger, BaseContext context, IFileService fileService)
        {
            _logger = logger;
            _context = context;
            _fileService = fileService;
        }

        /// <summary>
        /// Search city for city selection on order page
        /// </summary>
        /// <param name="search">Search string - search from start of string</param>
        /// <param name="lang">language of search and results</param>
        /// <returns>List of found cities or empty list</returns>
        [HttpGet]
        [Route("city")]
        public async Task<IActionResult> CitySearch([FromQuery] string search, string lang)
        {
            lang= lang.NormalizeLang();

            var found = await _context.Cities.Include(c => c.Name).ThenInclude(n => n.Translations).
                Include(c => c.Area).ThenInclude(a=>a.Name).ThenInclude(n=>n.Translations).
                Where(c => c.Name.Translations.Any(t => t.TranslationString.ToLower().StartsWith(search.ToLower()) && t.LanguageId == lang)).ToListAsync();

            var result = new List<dynamic>();

            foreach(var city in found)
            {

                result.Add(new
                {
                    id = city.Id,
                    name = city.Name.Content(lang),
                    area = city.Area.Name.Content(lang),
                });
            }

            return new JsonResult(new{ data = result });
        }

        /// <summary>
        /// Search available delivery companies in given city
        /// </summary>
        /// <param name="city_id">id of city</param>
        /// <param name="lang"></param>
        /// <returns>List of delivery companies or empty list</returns>
        [HttpGet]
        [Route("city-deliveries")]
        public async Task<IActionResult> GetDeliveries([FromQuery] string city_id, string lang)
        {
            lang = lang.NormalizeLang();

            if(int.TryParse(city_id,out int id))
            {
                var deliveries = await _context.Branches.Include(b=>b.Delivery).ThenInclude(d=>d.Name).ThenInclude(n=>n.Translations).Where(b=>b.BranchCityId == id).GroupBy(b=>b.Delivery).ToListAsync();

                var result = new List<dynamic>();

                foreach(var deliver in deliveries)
                {
                    result.Add(new
                    {
                        id = deliver.Key.Id,
                        name = deliver.Key.Name.Content(lang)
                    }) ;
                }

                return(new JsonResult(new {data=result}));

            }

            return BadRequest("Check parameters!");


        }

        /// <summary>
        /// Get all available branches of given delivery company in given city with desired limit
        /// </summary>
        /// <param name="lang">language</param>
        /// <param name="city_id">city id</param>
        /// <param name="delivery_id">delivery id</param>
        /// <param name="limit">limit count in list</param>
        /// <returns>List of delivery branches to select for delivery</returns>
        [HttpGet]
        [Route("delivery-branches")]
        public async Task<IActionResult> GetBranches([FromQuery] string lang, string city_id, string delivery_id, string limit )
        {
            lang = lang.NormalizeLang();

            if(int.TryParse (city_id,out int cty_id) && int.TryParse(delivery_id,out int del_id) && int.TryParse(limit,out int lim)) {
                
                var branches = await _context.Branches.
                    Include(b=>b.Description).ThenInclude(d=>d.Translations).
                    Include(b=>b.Address).ThenInclude(a=>a.Translations). 
                    Where(b=>b.DeliveryId == del_id && b.BranchCityId ==  cty_id).Take(lim).ToListAsync();

                var result = new List<dynamic>();

                foreach(var branch in branches)
                {
                    result.Add(new
                    {
                        id = branch.Id,
                        number = branch.LocalBranchNumber,
                        address = branch.Address.Content(lang),
                        description = branch.Description.Content(lang),
                        latitude = branch.Lat,
                        longtitude = branch.Long,
                        working_hours = branch.WorkHours
                    });
                }
                
                return Json(new { data = result });
            }

            return BadRequest("Check parameters!");
        }

    }
}
