using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Models;
using DiplomaMarketBackend.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomaMarketBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        ILogger<WorkController> _logger;
        BaseContext _context;
        IFileService _fileService;

        public SearchController(ILogger<WorkController> logger, BaseContext context, IFileService fileService)
        {
            _logger = logger;
            _context = context;
            _fileService = fileService;
        }

        /// <summary>
        /// Search in article title by given string
        /// </summary>
        /// <param name="search">search string</param>
        /// <param name="lang">search language</param>
        /// <param name="limit">results limit</param>
        /// <returns>List of found articles with id</returns>
        /// <response code="400">If the request values is bad</response>
        [HttpGet]
        [Route("main")]
        public async Task<IActionResult> MainSearch([FromQuery] string search, string lang, string limit)
        {
            lang = lang.NormalizeLang();

            if (int.TryParse(limit, out int quantity))
            {

                var words = search.Split(' ');

                var all_found = new List<ArticleModel>();

                foreach (var word in words)
                {
                    var found = await _context.Articles.Include(a => a.Title).ThenInclude(t => t.Translations).
                   Where(c => c.Title.Translations.Any(t => t.TranslationString.ToLower().Contains(word.ToLower()) && t.LanguageId == lang)).AsNoTracking().ToListAsync();

                    all_found.AddRange(found);
                }

                var groupped = all_found.GroupBy(a => a.Id).OrderByDescending(g => g.Count()).Take(quantity);

                var result = new List<dynamic>();

                foreach (var item in groupped)
                {
                    result.Add(new
                    {
                        id = item.First().Id,
                        title = item.First().Title.Content(lang),

                    });
                }

                return new JsonResult(new { data = result });

            }

            return BadRequest("Check parameters!");
        }
    }
}
