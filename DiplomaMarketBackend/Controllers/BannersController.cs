using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Models;
using DiplomaMarketBackend.Helpers;
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

        [HttpGet]
        [Route("category-banners")]
        [ResponseCache(VaryByQueryKeys = new[] { "category_id" }, Duration = 3600)]
        public async Task<IActionResult> GetCategoryBanners([FromQuery] int category_id = 0) {

            //Show banners from main if no in category
            if (category_id != 0 && _context.Banners.Where(b => b.CategoryId == category_id).Count() == 0) category_id = 0;

            var result = await _context.Banners.Where(b => b.CategoryId == category_id).Select(b => new { name = b.Name, url=Request.GetImageURL(BucketNames.banner, b.Url ) }).ToListAsync();

            return Json(result);
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddBanner([FromForm] int category_id, IFormFile file)
        {
            if(file != null) { 
            
                var url = await _fileService.SaveFileFromStream(BucketNames.banner.ToString(),file.FileName, file.OpenReadStream());

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
    }
}
