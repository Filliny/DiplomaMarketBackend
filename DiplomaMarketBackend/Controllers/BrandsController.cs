using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Models;
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
        public async Task<IActionResult> PopularBrands([FromQuery] string lang, int? number_to_show)
        {
            lang = lang.NormalizeLang();

            var max_art_brand = await _context.Brands.Include(b => b.Articles).
                OrderByDescending(b => b.Articles.Count()).
                Take(number_to_show ?? 100).ToListAsync();

            var result = new List<dynamic>();

            foreach (var brand in max_art_brand)
            {
                result.Add(new
                {
                    id = brand.Id,
                    name = brand.Name,
                    logo = Request.GetImageURL(BucketNames.logo, brand.LogoURL)

                });
            }

            return new JsonResult(new { data = result });
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
        public async Task<IActionResult> categoryBrands([FromQuery] int category_Id, int count, string lang)
        {
            lang = lang.NormalizeLang();

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == category_Id);

            if (category == null) return base.NotFound("No such category");

            //todo category-brands relation table with trigger refill

            var brands = _context.Articles.Include((System.Linq.Expressions.Expression<Func<ArticleModel, BrandModel>>)(a => a.Brand)).Where(a => a.TopCategoryId == category_Id).GroupBy((System.Linq.Expressions.Expression<Func<ArticleModel, BrandModel>>)(a => a.Brand)).ToList().Take(count);

            if (brands.Count() == 0) return base.NotFound("Category isn't top");

            var result = new List<dynamic>();

            foreach (var brand in brands)
            {

                if (brand.Key != null)
                {
                    result.Add(new
                    {
                        id = brand.Key.Id,
                        name = brand.Key.Name,
                        url = Request.GetImageURL(BucketNames.logo, brand.Key.LogoURL)
                    });
                }

            }

            return new JsonResult(new { data = result });
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateBrand([FromForm] Brand brand)
        {
            if (brand == null) return StatusCode(StatusCodes.Status400BadRequest, new Result
            {
                Status = "Error",
                Message = "Fill form!",
                Entity = brand
            });

            var new_brand = new BrandModel()
            {
                Name = brand.name,

            };

            if (brand.logo != null)
                new_brand.LogoURL = await _fileService.SaveFileFromStream(BucketNames.logo.ToString(), brand.logo.FileName, brand.logo.OpenReadStream());

            _context.Brands.Add(new_brand);
            _context.SaveChanges();
            brand.id = new_brand.Id;

            return StatusCode(StatusCodes.Status200OK, new Result
            {
                Status = "Success",
                Message = "Brand created",
                Entity = brand
            });
        }


        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> UpdateBrand([FromForm] Brand brand)
        {
            if (brand == null) return BadRequest("Fill form!");

            var ex_brand = _context.Brands.FirstOrDefault(b => b.Id == brand.id);

            if (ex_brand != null)
            {
                var old_logo = ex_brand.LogoURL;
                ex_brand.Name = brand.name;

                if (brand.logo != null)
                    ex_brand.LogoURL = await _fileService.SaveFileFromStream(BucketNames.logo.ToString(), brand.logo.FileName, brand.logo.OpenReadStream());

                _context.Brands.Update(ex_brand);
                _context.SaveChanges();

                if (old_logo != null)
                    _fileService.DeleteFile(BucketNames.logo.ToString(), old_logo);

                return StatusCode(StatusCodes.Status200OK, new Result
                {
                    Status = "Success",
                    Message = "Brand updated",
                    Entity = brand
                });

            }

            return StatusCode(StatusCodes.Status404NotFound, new Result
            {
                Status = "Error",
                Message = "Brand not found!"
            });

        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> RemoveBrand([FromQuery] int brand_id)
        {

            if (_context.Articles.Any(a => a.BrandId == brand_id)) return StatusCode(StatusCodes.Status500InternalServerError, new { Satus = "Failed", Message = "Brand have articles related - cant be removed!" });

            var brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == brand_id);
            if (brand != null)
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

                    return StatusCode(StatusCodes.Status500InternalServerError, new { Satus = "Error", Message = e.Message });
                }
            }

            return StatusCode(StatusCodes.Status404NotFound, new Result
            {
                Status = "Error",
                Message = "Brand not found!"
            });

        }


    }
}
