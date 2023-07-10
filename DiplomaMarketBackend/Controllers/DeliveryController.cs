using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Helpers;
using DiplomaMarketBackend.Parser.Article;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DiplomaMarketBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryController : Controller
    {
        ILogger<DeliveryController> _logger;
        BaseContext _context;
        IFileService _fileService;

        public DeliveryController(ILogger<DeliveryController> logger, BaseContext context, IFileService fileService)
        {
            _logger = logger;
            _context = context;
            _fileService = fileService;
        }

        /// <summary>
        /// To request list of delivery companies with id and name
        /// </summary>
        /// <param name="lang">language</param>
        /// <returns>list of delivery companies with id and name</returns>
        [HttpGet]
        //[Authorize(Roles = "User")]
        [Route("deliveries")]
        [ResponseCache(VaryByQueryKeys = new[] { "lang" }, Duration = 3600)]
        public async Task<IActionResult> DeliveriesIndex([FromQuery] string lang) 
        { 
            lang = lang.NormalizeLang();

            var deliveries = await _context.Deliveries.AsNoTracking().AsSplitQuery().
                Include(d=>d.Name.Translations).
                ToListAsync();

            var result = new List<dynamic>();
            foreach ( var deliver in deliveries)
            {
                result.Add(new
                {
                    id = deliver.Id,
                    name = deliver.Name?.Content(lang),
                });
            }
            return new JsonResult(new {data=result});

        }

        /// <summary>
        /// Search city for city selection on order page
        /// </summary>
        /// <param name="search">Search string - search from start of string</param>
        /// <param name="lang">language of search and results</param>
        /// <param name="limit">Number of rows to get - default 10</param>
        /// <returns>List of found cities or empty list</returns>
        [HttpGet]
        [Route("city")]
        public async Task<IActionResult> CitySearch([FromQuery] string? search, string lang, int limit=10)
        {
            lang= lang.NormalizeLang();
            if (search.IsNullOrEmpty()) search = "";

            var found = await _context.Cities.AsNoTracking().AsSplitQuery().
                Include(c => c.Name.Translations).
                Include(c => c.Area.Name.Translations).
                Where(c => c.Name.Translations.Any(t => t.TranslationString.ToLower().StartsWith(search.ToLower()) && t.LanguageId == lang)).
                OrderBy(c=>c.Name.OriginalText).
                Take(limit).ToListAsync();

            var result = new List<dynamic>();

            foreach(var city in found)
            {

                result.Add(new
                {
                    id = city.Id,
                    name = city.Name?.Content(lang),
                    area = city.Area?.Name?.Content(lang),
                    coautsu = city.CoatsuCode,
                    zip = city.Index1
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
        public async Task<IActionResult> GetDeliveries([FromQuery] int city_id, string lang)
        {
            lang = lang.NormalizeLang();

            var deliveries = await _context.Branches.AsNoTracking().AsSplitQuery().
               Include(b=>b.Address.Translations).
               Include(b=>b.Description.Translations).
                Where(b=>b.BranchCityId == city_id).
                GroupBy(b=>b.Delivery).
                ToListAsync();

            var result = new List<dynamic>();

            foreach(var deliver in deliveries)
            {
                var delivery = _context.Deliveries.AsNoTracking().Include(d => d.Name.Translations)
                    .FirstOrDefault(d => d.Id == deliver.Key.Id);

                var branches = new List<dynamic>();

                foreach (var branch in deliver)
                {
                  branches.Add(new
                  {
                      id=branch.Id,
                      local_number = branch.LocalBranchNumber,
                      address = branch.Address.Content(lang),
                      description = branch.Description.Content(lang),
                      working_hours = branch.WorkHours
                  });
                }
                
                result.Add(new
                {
                    id = deliver.Key?.Id,
                    name = delivery?.Name?.Content(lang),
                    branches
                }) ;
                
            }

            return(new JsonResult(new {data=result}));

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
        public async Task<IActionResult> GetBranches([FromQuery] string lang, int city_id, int delivery_id, int limit=10 )
        {
            lang = lang.NormalizeLang();


            var branches = await _context.Branches.AsNoTracking().AsSplitQuery().
                Include(b=>b.Description).ThenInclude(d=>d.Translations).
                Include(b=>b.Address).ThenInclude(a=>a.Translations). 
                Where(b=>b.DeliveryId == delivery_id && b.BranchCityId ==  city_id).Take(limit).ToListAsync();

            var result = new List<dynamic>();

            foreach(var branch in branches)
            {
                result.Add(new
                {
                    id = branch.Id,
                    number = branch.LocalBranchNumber,
                    address = branch.Address?.Content(lang),
                    description = branch.Description?.Content(lang),
                    latitude = branch.Lat,
                    longtitude = branch.Long,
                    working_hours = branch.WorkHours
                });
            }
            
            return Json(new { data = result });
        }

    }
}
