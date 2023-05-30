using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomaMarketBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActionsController : ControllerBase
    {
        ILogger<GoodsController> _logger;
        BaseContext _context;
        IFileService _fileService;

        public ActionsController(ILogger<GoodsController> logger, BaseContext context, IFileService fileService)
        {
            _logger = logger;
            _context = context;
            _fileService = fileService;
        }

        /// <summary>
        /// Get list of existing actions
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("actions")]
        public async Task<IActionResult> ActionsList([FromQuery] string lang)
        {
            var actions = await _context.Actions.
                Include(a=>a.Articles).
                Include(a=>a.Name.Translations).
                Include(a=>a.Description.Translations).
                Select(a => new
            {
                id = a.Id,
                name = a.Name.Content(lang),
                description = a.Description.Content(lang),
                articles_in = a.Articles.Count(),
                banner_big = Request.GetImageURL(BucketNames.banner,a.BannerBig),
                banner_small = Request.GetImageURL(BucketNames.banner, a.BannerSmall)

                }).ToListAsync();

            return Ok(actions);
        }

        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> GetAction([FromQuery] int action_id)
        {
            return Ok();
        }
    }
}
