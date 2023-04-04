using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomaMarketBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : Controller
    {

        ILogger<WorkController> _logger;
        ICloudStorageService _storageService;
        BaseContext _context;
        IFileService _fileService;

        public CategoriesController(ILogger<WorkController> logger, ICloudStorageService cloudStorageService, BaseContext context, IFileService fileService)
        {
            _logger = logger;
            _storageService = cloudStorageService;
            _context = context;
            _fileService = fileService;
        }

        /// <summary>
        /// Request fo retrieve all categories tree 
        /// </summary>
        /// <param name="lang">Language short abbreviation e.g. ru or uk</param>
        /// <returns>categories tree in json format</returns>
        /// <remarks>
        /// Simple category entity
        /// 
        ///       {
        ///            "id": 0,
        ///            "name": "Ноутбуки",
        ///            "parentId": null,
        ///            "children": []
        ///        },
        /// 
        /// </remarks>
        [Route("full")]
        [HttpGet]
        [Produces("application/json")]
        [ResponseCache(VaryByQueryKeys = new[] {"lang"}, Duration = 30)]
        public IActionResult GetFullTree([FromQuery] string lang)
        {
            if (lang.ToUpper() != "UK" && lang.ToUpper() != "RU")
                lang = "UA";

            var top_categories = _context.Categories
                 .Include(c => c.Name)
                 .ThenInclude(n => n.Translations)
                 .Where(c => c.ParentCategoryId == null)
                 .ToList();

            var result = new List<Models.Category>();

            foreach (var category in top_categories)
            {
                var output_cat = new Models.Category();

                if (category.Name != null)
                {
                    output_cat.Name = category.Name.Translations.FirstOrDefault(t => t.LanguageId == lang.ToUpper()) != null ?
                    category.Name.Translations.First(t => t.LanguageId == lang.ToUpper()).TranslationString :
                    category.Name.OriginalText;
                    output_cat.Id = category.Id;
                }

                var mid_childs = _context.Categories.Include(c => c.Name).ThenInclude(n => n.Translations).Where(c => c.ParentCategoryId == category.Id).ToList();

                foreach (var child in mid_childs)
                {
                    var output_mchild = new Models.Category();
                    if (child.Name != null)
                    {
                        output_mchild.Name = child.Name.Translations.FirstOrDefault(t => t.LanguageId == lang.ToUpper()) != null ?
                        child.Name.Translations.FirstOrDefault(t => t.LanguageId == lang.ToUpper()).TranslationString :
                        child.Name.OriginalText;
                        output_mchild.Id = child.Id;
                    }

                    var low_childs = _context.Categories.Include(c => c.Name).ThenInclude(n => n.Translations).Where(c => c.ParentCategoryId == child.Id).ToList();

                    foreach (var lowchild in low_childs)
                    {
                        var output_lchild = new Models.Category();
                        if (child.Name != null)
                        {
                            output_lchild.Name = lowchild.Name.Translations.FirstOrDefault(t => t.LanguageId == lang.ToUpper()) != null ?
                            lowchild.Name.Translations.FirstOrDefault(t => t.LanguageId == lang.ToUpper()).TranslationString :
                            lowchild.Name.OriginalText;
                            output_lchild.Id = lowchild.Id;
                        }

                        output_mchild.Children.Add(output_lchild);
                    }

                    output_cat.Children.Add(output_mchild);
                }

                result.Add(output_cat);
            }

            return new JsonResult(new { data = result });
        }
    }
}
