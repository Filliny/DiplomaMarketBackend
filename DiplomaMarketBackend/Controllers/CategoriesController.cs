using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

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
        [ResponseCache(VaryByQueryKeys = new[] {"lang"}, Duration = 3600)]
        public IActionResult GetFullTree([FromQuery] string lang)
        {
            lang= lang.NormalizeLang();

            var top_categories = _context.Categories
                 .Include(c => c.Name)
                 .ThenInclude(n => n.Translations)
                 .Where(c => c.ParentCategoryId == null)
                 .ToList();

            var result = new List<Models.Category>();

            string loge = "\n\n\nCategory tree\n\n\n";
            var baseUrl = Request.Scheme + "://" + Request.Host + "/api/Goods/category/";

            foreach (var category in top_categories)
            {
                var output_cat = new Models.Category();

                if (category.Name != null)
                {
                    output_cat.Name = category.Name.Content(lang);
                    output_cat.Id = category.Id;
                    output_cat.SmallIcon = category.ImgData != null ? Convert.ToBase64String(category.ImgData) : null;
                    output_cat.BigPicture = category.ImgUrl != ""? baseUrl + category.ImgUrl + ".jpg":null;
                    loge += "\n"+ category.Name.Content(lang);
                }

                var mid_childs = _context.Categories.Include(c => c.Name).ThenInclude(n => n.Translations).Where(c => c.ParentCategoryId == category.Id).ToList();

                foreach (var child in mid_childs)
                {
                    var output_mchild = new Models.Category();
                    if (child.Name != null)
                    {
                        output_mchild.Name = child.Name.Content(lang);
                        output_mchild.Id = child.Id;
                        output_mchild.SmallIcon = child.ImgData != null ? Convert.ToBase64String(child.ImgData) : null;
                        output_mchild.BigPicture = child.ImgUrl != ""?baseUrl + child.ImgUrl + ".jpg":null;
                        loge += "\n--"+ child.Name.Content(lang);
                    }

                    var low_childs = _context.Categories.Include(c => c.Name).ThenInclude(n => n.Translations).Where(c => c.ParentCategoryId == child.Id).ToList();

                    foreach (var lowchild in low_childs)
                    {
                        var output_lchild = new Models.Category();
                        if (child.Name != null)
                        {
                            output_lchild.Name = lowchild.Name.Content(lang);
                            output_lchild.Id = lowchild.Id;
                            output_lchild.SmallIcon = lowchild.ImgData != null ? Convert.ToBase64String(lowchild.ImgData) : null;
                            output_lchild.BigPicture = lowchild.ImgUrl != "" ? baseUrl + lowchild.ImgUrl + ".jpg" : null;
                            loge += "\n----"+ lowchild.Name.Content(lang);
                        }

                        output_mchild.Children.Add(output_lchild);
                    }

                    output_cat.Children.Add(output_mchild);
                }

                //_logger.LogInformation(loge);

                result.Add(output_cat);
            }

            return new JsonResult(new { data = result });
        }
    }
}
