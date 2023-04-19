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

        /// <summary>
        /// Get popular brands
        /// </summary>
        /// <param name="lang">language</param>
        /// <param name="number_to_show">number of brands to show (not more than 100)</param>
        /// <returns>List of most popular brands</returns>
        /// <response code="400">If the request values is bad</response>
        [HttpGet]
        [Route("popular")]
        public async Task<IActionResult> PopularBrands([FromQuery] string lang, string number_to_show)
        {
            lang = lang.NormalizeLang();

            if(int.TryParse(number_to_show,out int quantity))
            {
                if(quantity > 100) { quantity = 100; }

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

        /// <summary>
        /// Get list of top category brands 
        /// </summary>
        /// <param name="category_Id">category id</param>
        /// /// <param name="count">number items to show</param>
        /// <param name="lang"></param>
        /// <returns>List of brands presents in this category</returns>
        /// <response code="400">If the request values is bad</response>
        /// <response code="404">If category isn't top and haven't brands relation</response>
        [HttpGet]
        [Route("category-brands")]
        public async Task<IActionResult> categoryBrands([FromQuery] string category_Id, string count, string lang)
        {
            lang = lang.NormalizeLang();

            if(int.TryParse(category_Id,out int cat_id) && int.TryParse(count,out int quantity))
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == cat_id);

                if(category == null) return NotFound("No such category");

                //todo category-brands relation table with trigger refill

                var brands = _context.Articles.Include(a=>a.Brand).Where(a=>a.TopCategoryId ==  cat_id).GroupBy(a=>a.Brand).ToList().Take(quantity);

                if (brands.Count() == 0) return NotFound("Category isn't top");

                var result = new List<dynamic>();

                foreach(var brand in brands) {

                    if(brand.Key != null)
                    {
                        result.Add(new
                        {
                            id= brand.Key.Id,
                            name = brand.Key.Name,
                            url = Request.GetImageURL(BucketNames.logo, brand.Key.LogoURL)
                        });
                    }

                }

                return new JsonResult(new {data = result});
            }

            return BadRequest("Check parameters!");
        }
    }
}
