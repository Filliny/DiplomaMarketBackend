using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomaMarketBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandsController : Controller
    {
        ILogger<WorkController> _logger;
        BaseContext _context;
        IFileService _fileService;

        public BrandsController(ILogger<WorkController> logger, BaseContext context, IFileService fileService)
        {
            _logger = logger;
            _context = context;
            _fileService = fileService;
        }

        [HttpGet]
        [Route("popular")]
        public async Task<IActionResult> PopularBrands([FromQuery] string lang, string number_to_show)
        {
            lang = lang.NormalizeLang();

            if(int.TryParse(number_to_show,out int quantity))
            {
               var max_art_brand = await _context.Brands.Include(b=>b.Articles).
                    OrderByDescending(b=>b.Articles.Count()).
                    Take(quantity).ToListAsync();

                var result = new List<dynamic>();
                
                foreach(var brand in max_art_brand)
                {
                    result.Add(new
                    {
                        id = brand.Id,
                        name = brand.Name,
                        logo = Request.GetImageURL(BucketNames.logo, brand.LogoURL)

                    });
                }

                return new JsonResult(new {data = result});
            }

            return BadRequest("Check parameters!");

        }
    }
}
