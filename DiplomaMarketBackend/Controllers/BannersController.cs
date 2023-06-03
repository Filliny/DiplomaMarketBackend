using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Models;
using DiplomaMarketBackend.Helpers;
using DiplomaMarketBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomaMarketBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannersController : Controller
    {
        ILogger<GoodsController> _logger;
        BaseContext _context;
        IFileService _fileService;

        public BannersController(ILogger<GoodsController> logger, BaseContext context, IFileService fileService)
        {
            _logger = logger;
            _context = context;
            _fileService = fileService;
        }

        /// <summary>
        /// Get banners for given category
        /// </summary>
        /// <param name="category_id">Category id</param>
        /// <returns>List of banners url for category if any is set, othervice list for main page </returns>
        [HttpGet]
        [Route("category-banners")]
        [ResponseCache(VaryByQueryKeys = new[] { "category_id" }, Duration = 3600)]
        public async Task<IActionResult> GetCategoryBanners([FromQuery] int category_id = 0) {

            //Show banners from main if no in category
            if (category_id != 0 && _context.Banners.Where(b => b.CategoryId == category_id).Count() == 0) category_id = 0;

            var result = await _context.Banners.Where(b => b.CategoryId == category_id).Select(b => new { id = b.Id, name = b.Name, url=Request.GetImageURL(BucketNames.banner, b.Url ) }).ToListAsync();

            return Json(result);
        }///

        /// <summary>
        /// Get list of categories with banners and banners quantity
        /// </summary>
        /// <returns>Reurns list of categories with list of banners</returns>
        [HttpGet]
        [Route("banners-list")]
        public async Task<IActionResult> GetListBanners()
        {
            var banners = await _context.Banners.GroupBy(b => b.CategoryId).ToListAsync();

            var result = new List<dynamic>();

            foreach (var b in banners)
            {
                var category = await _context.Categories.Include(c => c.Name.Translations).FirstOrDefaultAsync(c => c.Id == b.Key) ?? new CategoryModel
                {
                    Id = 0,
                    Name = new TextContent { OriginalText = "MainPage" }
                };

                var cat = new
                {
                    category_id = category.Id,
                    name = category.Name.OriginalText,
                    count = b.Count(),
                    banners = b.Select(b => new { id = b.Id, name = b.Name, url = Request.GetImageURL(BucketNames.banner,b.Url) }).ToList(),
                };
                result.Add(cat);
            }

            return Ok(result);
        }

        /// <summary>
        /// Upload banner for category
        /// </summary>
        /// <param name="category_id">Category id - 0 for main page</param>
        /// <param name="file">Form file</param>
        /// <returns>Ok if success</returns>
        /// <response code="400">If file is empty</response>
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddBanner([FromForm] int category_id, IFormFile file)
        {
            if (file != null)
            {

                var url = await _fileService.SaveFileFromStream(BucketNames.banner.ToString(), file.FileName, file.OpenReadStream());

                var banner = new BannerModel
                {
                    CategoryId = category_id,
                    Name = "Main category banner name",
                    Url = url

                };

                _context.Banners.Add(banner);
                _context.SaveChanges();

                return Ok();

            }

            return BadRequest();
        }

        /// <summary>
        /// Remove banner from category
        /// </summary>
        /// <param name="banner_id">Banner Id</param>
        /// <returns>Ok if success</returns>
        /// <response code="400">If banner not found</response>
        [HttpDelete]
        [Route("remove")]
        public async Task<IActionResult> removeBanner([FromQuery] int banner_id)
        {
            var banner_img = await _context.Banners.FirstOrDefaultAsync(b => b.Id == banner_id);

            if (banner_img != null)
            {
                _fileService.DeleteFile(BucketNames.banner.ToString(), banner_img.Url);
                _context.Banners.Remove(banner_img);
                _context.SaveChanges();

                return Ok(new Result
                {
                    Status = "Success",
                    Message = "Banner removed!"
                });
            }

            return BadRequest(new Result
            {
                Status="Error",
                Message= "Banner img not found!"
            });
        }

    }
}
