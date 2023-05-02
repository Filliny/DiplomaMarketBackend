using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Models;
using DiplomaMarketBackend.Entity.Models.Delivery;
using DiplomaMarketBackend.Helpers;
using DiplomaMarketBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace DiplomaMarketBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandsController : ControllerBase
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

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateBrand([FromForm] Brand brand)
        {
            if (brand == null) return BadRequest("Fill form!");

            var new_brand = new BrandModel()
            {
                Name = brand.name,

            };

            if (brand.logo != null)
                new_brand.LogoURL = await _fileService.SaveFileFromStream(BucketNames.logo.ToString(), brand.logo.FileName, brand.logo.OpenReadStream());

            _context.Brands.Add(new_brand);
            _context.SaveChanges();
            brand.id = new_brand.Id;

            return Ok(brand);
        }


        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> UpdateBrand([FromForm] Brand brand)
        {
            if (brand == null) return BadRequest("Fill form!");

            var  ex_brand = _context.Brands.FirstOrDefault(b=>b.Id == brand.id);
  
            if(ex_brand != null)
            {
                var old_logo = ex_brand.LogoURL;
                ex_brand.Name = brand.name;

                if(brand.logo  != null)
                    ex_brand.LogoURL = await _fileService.SaveFileFromStream(BucketNames.logo.ToString(), brand.logo.FileName, brand.logo.OpenReadStream());

                _context.Brands.Update(ex_brand);
                _context.SaveChanges();

                if(old_logo != null)
                _fileService.DeleteFile(BucketNames.logo.ToString(), old_logo);

                return Ok(brand);

            }

            return NotFound("No such brand !");

        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> RemoveBrand([FromQuery] string brand_id)
        {
            if(int.TryParse(brand_id, out var id))
            {
                if (_context.Articles.Any(a => a.BrandId == id)) return StatusCode(StatusCodes.Status500InternalServerError, new { Satus = "Failed", Message = "Brand have articles related - cant be removed!" });

                var brand = _context.Brands.FirstOrDefault(b=>b.Id ==  id);
                if(brand != null)
                {
                    try
                    {
                        _context.Brands.Remove(brand);
                        _context.SaveChanges();
                        _fileService.DeleteFile(BucketNames.logo.ToString(), brand.LogoURL);

                        return Ok(brand);
                }
                    catch (Exception e)
                    {

                        return StatusCode(StatusCodes.Status500InternalServerError, new { Satus = "Failed", Message = e.Message });
                    }
                }

                return NotFound("Brand not found");

            }

            return BadRequest("Check id!");

        }


    }
}
