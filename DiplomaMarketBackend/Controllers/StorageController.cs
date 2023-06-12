using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomaMarketBackend.Controllers;

[Controller]
[Route("api/[controller]")]
public class StorageController:ControllerBase
{
    ILogger<StorageController> _logger;
    BaseContext _context;
    IFileService _fileService;

    public StorageController(ILogger<StorageController> logger, BaseContext context, IFileService fileService)
    {
        _logger = logger;
        _context = context;
        _fileService = fileService;
    }

    /// <summary>
    /// List of articles for ordering to refill
    /// Товари на поповнення
    /// </summary>
    /// <param name="lang"></param>
    /// <param name="search"></param>
    /// <param name="category_id"></param>
    /// <param name="page"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("refill_list")]
    public async Task<JsonResult> GetRefillList([FromQuery]string lang, string? search, int? category_id, int page=1, int limit=10 )
        {
            lang = lang.NormalizeLang();

            var articles = new List<dynamic>();

            var goods = _context.Articles.AsNoTracking().AsSplitQuery().
                Include(a => a.Title.Translations).
                Include(a => a.Category).
                Include(a=>a.Images).ThenInclude(p=>p.small).
                Include(a=>a.Category.Name.Translations).
                Where(a=>a.Status != "available" && a.Status != "awaiting" );

            if (category_id is not null)
            {
                goods = goods.Where(a => a.CategoryId == category_id);
            }
            
            if (search is not null)
            {
                goods = goods.Where(c => c.Title.Translations.Any(t => t.TranslationString.ToLower().Contains(search.ToLower()) && t.LanguageId == lang));
            }

            var goodsList = await goods.ToListAsync();

            int total_goods = goodsList.Count;
            int total_pages = (int)Math.Ceiling((decimal)total_goods / (decimal)limit);
            
            if (page > total_pages) page = total_pages;

            int skip = (page - 1) * limit;

            goodsList = goodsList.Skip(skip).Take(limit).ToList();

            foreach (var article in goodsList)
            {
                articles.Add(new
                {
                    id= article.Id,
                    name = article.Title.Content(lang),
                    price = article.Price,
                    category = article.Category.Name.Content(lang),
                    preview = Request.GetImageURL(BucketNames.small.ToString(),article.Images.First().small.url??""),
                    quantity = article.Quantity,
                    status = article.Status
                });
            }

            var result = new
            {
                data = new
                {
                    articles,
                    total_goods,
                    total_pages,
                    displayed_page = page
                }
            };

            return new JsonResult(result);
            
        }
}