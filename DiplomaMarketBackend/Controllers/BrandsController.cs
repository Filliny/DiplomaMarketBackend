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
        ILogger<BrandsController> _logger;
        BaseContext _context;
        IFileService _fileService;

        public BrandsController(ILogger<BrandsController> logger, BaseContext context, IFileService fileService)
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



        /// <summary>
        /// Get list of category brands for filter
        /// </summary>
        /// <param name="category_Id">category id</param>
        /// /// <param name="count">number items to show</param>
        /// <param name="lang"></param>
        /// <returns>List of brands presents in this category</returns>
        /// <response code="400">If the request values is bad</response>
        /// <response code="404">If category isn't top and haven't brands relation</response>
        [HttpGet]
        [Route("filter-brands")]
        public async Task<IActionResult> categoryFilterBrands([FromQuery] int category_Id, string lang, int count = 999999)
        {
            lang = lang.NormalizeLang();

            var category = await _context.Categories.
                Include(c => c.Brands).
                FirstOrDefaultAsync(c => c.Id == category_Id);

            if (category == null) return base.NotFound("No such category");

            //todo category-brands relation table with trigger refill

            //var brands = _context.Articles.Include((System.Linq.Expressions.Expression<Func<ArticleModel, BrandModel>>)(a => a.Brand)).Where(a => a.CategoryId == category_Id).GroupBy((System.Linq.Expressions.Expression<Func<ArticleModel, BrandModel>>)(a => a.Brand)).ToList().Take(count);

            var new_brands = category.Brands;

            var result = new List<dynamic>();

            foreach (var brand in new_brands)
            {

                if (brand != null)
                {
                    result.Add(new
                    {
                        id = brand.Id,
                        name = brand.Name,
                    });
                }

            }

            return new JsonResult(new { data = result });
        }


        [HttpGet]
        [Route("brands-service")]
        public async Task<IActionResult> GetBrandsServices([FromQuery] char search_symbol, string lang)
        {
            lang = lang.NormalizeLang();

            var brands = await _context.Brands.Include(c => c.Categories).ThenInclude(c => c.Name.Translations).
                Where(b => b.Name.StartsWith(search_symbol.ToString().ToUpper())).ToListAsync();

            var all_services = await _context.Services.Include(s => s.City.Name.Translations).
                Include(s => s.Brand).Where(s => brands.Contains(s.Brand)).ToListAsync();

            var result = new List<dynamic>();
            foreach (var brand in brands)
            {
                var out_brand = new
                {
                    id = brand.Id,
                    name = brand.Name,
                    categories = new List<dynamic>()
                };

                foreach (var category in brand.Categories)
                {
                    var out_category = new
                    {
                        id = category.Id,
                        name = category.Name.Content(lang),
                        services = new List<dynamic>()
                    };

                    var services = all_services.Where(s => s.BrandId == brand.Id && s.CategoryId == category.Id).ToList();

                    foreach (var service in services)
                    {
                        var out_service = new
                        {
                            id = service.Id,
                            name = service.Name,
                            city = service.City.Name.Content(lang),
                            address = service.Address,
                            phone = service.Phone,
                        };
                        out_category.services.Add(out_service);
                    }
                    out_brand.categories.Add(out_category);
                }
                result.Add(out_brand);
            }

            return new JsonResult(result);
        }

        /// <summary>
        /// Create new brand
        /// </summary>
        /// <param name="brand">Brand data from form</param>
        /// <returns>Result entity with Status, Message and Entity with id if success</returns>
        /// <response code="400">If the request values is bad</response>
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<Brand>> CreateBrand([FromForm] Brand brand)
        {
            if (brand == null) return BadRequest(new Result
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

            return Ok(new Result
            {
                Status = "Success",
                Message = "Brand created",
                Entity = brand
            });
        }

        /// <summary>
        /// Update existing brand by name
        /// </summary>
        /// <param name="brand">Form data for brand</param>
        /// <returns>Result entity with Status, Message and Entity with id if success</returns>
        /// <response code="400">If the request values is bad</response>
        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> UpdateBrand([FromForm] Brand brand)
        {
            if (brand == null) return BadRequest(new Result
            {
                Status = "Error",
                Message = "Fill form!",
                Entity = brand
            });

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

                return Ok(new Result
                {
                    Status = "Success",
                    Message = "Brand updated",
                    Entity = brand
                });

            }

            return NotFound(new Result
            {
                Status = "Error",
                Message = "Brand not found!"
            });

        }

        /// <summary>
        /// Delete brand by id
        /// </summary>
        /// <param name="brand_id">brand id</param>
        /// <returns>Ok if removed sucessfully</returns>
        /// <response code="400">If the request values is bad or brand cant be removed</response>
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

                    return Ok(new Result
                    {
                        Status = "Sucess",
                        Message = "Brand removed!"
                    });
                }
                catch (Exception e)
                {

                    return BadRequest(new { Satus = "Error", Message = e.Message });
                }
            }

            return NotFound(new Result
            {
                Status = "Error",
                Message = "Brand not found!"
            });

        }


    }
}
