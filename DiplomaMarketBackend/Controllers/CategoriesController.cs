using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DiplomaMarketBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[EnableCors("AllowOrigin")]
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
        [ResponseCache(VaryByQueryKeys = new[] { "lang" }, Duration = 3600)]
        public IActionResult GetFullTree([FromQuery] string lang)
        {
            lang = lang.NormalizeLang();

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
                    output_cat.BigPicture = category.ImgUrl != "" ? baseUrl + category.ImgUrl + ".jpg" : null;
                    loge += "\n" + category.Name.Content(lang);
                }

                var mid_childs = _context.Categories.Include(c => c.Name).ThenInclude(n => n.Translations).Where(c => c.ParentCategoryId == category.Id).ToList();

                //categories adittionally showed in parent
                var additions = _context.Categories.Include(c => c.Name).ThenInclude(n => n.Translations).Where(c => c.ShowInCategoryId == category.Id).ToList();

                mid_childs.AddRange(additions);


                foreach (var child in mid_childs)
                {
                    var output_mchild = new Models.Category();
                    if (child.Name != null)
                    {
                        output_mchild.Name = child.Name.Content(lang);
                        output_mchild.Id = child.Id;
                        output_mchild.SmallIcon = child.ImgData != null ? Convert.ToBase64String(child.ImgData) : null;
                        output_mchild.BigPicture = child.ImgUrl != "" ? baseUrl + child.ImgUrl + ".jpg" : null;
                        loge += "\n--" + child.Name.Content(lang);
                    }

                    var low_childs = _context.Categories.Include(c => c.Name).ThenInclude(n => n.Translations).Where(c => c.ParentCategoryId == child.Id).ToList();

                    //categories adittionally showed in parent
                    var additions_l = _context.Categories.Include(c => c.Name).ThenInclude(n => n.Translations).Where(c => c.ShowInCategoryId == child.Id).ToList();

                    low_childs.AddRange(additions_l);

                    foreach (var lowchild in low_childs)
                    {
                        var output_lchild = new Models.Category();
                        if (child.Name != null)
                        {
                            output_lchild.Name = lowchild.Name.Content(lang);
                            output_lchild.Id = lowchild.Id;
                            output_lchild.SmallIcon = lowchild.ImgData != null ? Convert.ToBase64String(lowchild.ImgData) : null;
                            output_lchild.BigPicture = lowchild.ImgUrl != "" ? baseUrl + lowchild.ImgUrl + ".jpg" : null;
                            loge += "\n----" + lowchild.Name.Content(lang);
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



        /// <summary>
        /// Get sub-categories of given top category
        /// </summary>
        /// <param name="category_Id">Top category id</param>
        /// <param name="lang">language</param>
        /// <returns>List of sub-categories with images and inner categories list, also include breadcrumbs to top category</returns>
        [HttpGet]
        [Route("get-sub")]
        public async Task<IActionResult> GetSubCategories([FromQuery] string category_Id, string lang)
        {
            lang= lang.NormalizeLang();

            var subcategories = new List<dynamic>();
            var breadcrumbs = new List<dynamic>();

            if (int.TryParse(category_Id, out int id))
            {
                var output_cat = await _context.Categories.
                    Include(c=>c.ChildCategories).ThenInclude(c=>c.Name).ThenInclude(c=>c.Translations).
                    Include(c=>c.Name).ThenInclude(n=>n.Translations).
                    Where(c=>c.ParentCategoryId == id).ToListAsync();

                var add_cat = await _context.Categories.
                    Include(c => c.ChildCategories).ThenInclude(c => c.Name).ThenInclude(c => c.Translations).
                    Include(c => c.Name).ThenInclude(n => n.Translations).
                    Where(c => c.ShowInCategoryId == id).ToListAsync();

                output_cat.AddRange(add_cat);

                foreach (var cat in output_cat)
                {
                    if (cat.ImgUrl.IsNullOrEmpty()) continue;

                    var head = new
                    {
                        id = cat.Id,
                        name = cat.Name.Content(lang),
                        big_picture = Request.GetImageURL(BucketNames.category, cat.ImgUrl),
                        childrens = new List<dynamic>()

                    };

                    if(cat.ChildCategories !=  null)
                    {
                        foreach(var cat2 in cat.ChildCategories)
                        {
                            if (cat2.ImgUrl.IsNullOrEmpty()) continue;

                            var child = new
                            {
                                id = cat2.Id,
                                name = cat2.Name.Content(lang),
                                big_picture = Request.GetImageURL(BucketNames.category, cat2.ImgUrl),
                            };

                            head.childrens.Add(child);  
                        }
                    }

                    subcategories.Add(head);

                }

                int? curr_id = id;

                do
                {
                    var parent_cat = _context.Categories.
                        Include(c => c.Name).ThenInclude(n => n.Translations).
                        FirstOrDefault(c => c.Id == curr_id);

                    var newcat = new
                    {
                        id = parent_cat.Id,
                        name = parent_cat.Name.Content(lang),
                    };
                    breadcrumbs.Add(newcat);

                    curr_id = parent_cat.ParentCategoryId;

                } while (curr_id != null);

                

                return new JsonResult(new { data = subcategories, breadcrumbs = breadcrumbs });
            }



            return Ok();
        }
    }
}
