using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Models;
using DiplomaMarketBackend.Helpers;
using DiplomaMarketBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

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

            var result = new List<Models.OutCategory>();

            string loge = "\n\n\nCategory tree\n\n\n";
            var baseUrl = Request.Scheme + "://" + Request.Host + "/api/Goods/category/";

            foreach (var category in top_categories)
            {
                var output_cat = new Models.OutCategory();

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
                    var output_mchild = new Models.OutCategory();
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
                        var output_lchild = new Models.OutCategory();
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
            lang = lang.NormalizeLang();

            var subcategories = new List<dynamic>();
            var breadcrumbs = new List<dynamic>();

            if (int.TryParse(category_Id, out int id))
            {
                var output_cat = await _context.Categories.
                    Include(c => c.ChildCategories).ThenInclude(c => c.Name).ThenInclude(c => c.Translations).
                    Include(c => c.Name).ThenInclude(n => n.Translations).
                    Where(c => c.ParentCategoryId == id).ToListAsync();

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

                    if (cat.ChildCategories != null)
                    {
                        foreach (var cat2 in cat.ChildCategories)
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

        [HttpGet]
        [Route("flat")]
        public async Task<IActionResult> GetFlatList([FromQuery] string? search, string lang, string limit, string page)
        {
            lang = lang.NormalizeLang();
            //List<CategoryModel> categories;

            if (int.TryParse(limit, out int quantity) && int.TryParse(page, out int page_num))
            {
                List<CategoryModel> categories;

                if (search != null)
                {
                    categories = _context.Categories.Include(c => c.Name.Translations).Where(c => c.Name.Translations.Any(t => t.TranslationString.ToLower().Contains(search.ToLower()) && t.LanguageId == lang)).ToList();
                }
                else
                {
                    categories = _context.Categories.Include(c => c.Name.Translations).ToList();
                }

                var found = categories.Count();

                int total_pages = (int)Math.Ceiling((decimal)found / (decimal)quantity);

                if (page_num == 0) page_num = 1;
                if (page_num > total_pages) page_num = total_pages;

                int skip = (page_num - 1) * quantity;

                var found_cat = categories.Skip(skip).Take(quantity).ToList();


                var result = new List<dynamic>();
                foreach (var category in found_cat)
                {
                    result.Add(new
                    {

                        id = category.Id,
                        name = category.Name.Content(lang),

                    });
                }


                var response = new
                {
                    data = result,
                    found,
                    total_pages,
                    current_page = page_num
                };

                return new JsonResult(response);

            }
            return BadRequest("Check parameters!");

        }


        [HttpGet]
        [Route("category")]
        public async Task<IActionResult> GetOneCategory([FromQuery] string category_Id, string lang)
        {
            lang = lang.NormalizeLang();

            if (int.TryParse(category_Id, out var catId))
            {

                var category = await _context.Categories.Include(c => c.ParentCategory.Name.Translations).Include(c => c.Name.Translations).Include(c => c.ChildCategories).ThenInclude(c => c.Name.Translations).FirstOrDefaultAsync(c => c.Id == catId);

                if (category == null) return NotFound("No such category!");

                var result = new
                {
                    id = category_Id,
                    name = category.Name.Content(lang),
                    big_picture = category.ImgUrl == null ? null : Request.GetImageURL(BucketNames.category, category.ImgUrl),
                    parent = category.ParentCategory == null ? null : new
                    {
                        id = category.ParentCategory.Id,
                        name = category.ParentCategory.Name.Content(lang),
                    },
                    children = new List<dynamic>()
                };

                foreach (var subcategory in category.ChildCategories)
                {
                    result.children.Add(new
                    {
                        id = subcategory.Id,
                        name = subcategory.Name.Content(lang),
                    });
                }

                return new JsonResult(new { data = result });
            }

            return BadRequest("Check parameters!");

        }

        /// <summary>
        /// Parent candidates categories
        /// </summary>
        /// <param name="category_id">category id for move category (excludes from result as well as childs) can be null</param>
        /// <param name="lang">language</param>
        /// <returns>Return list of categories as candidates for parent categories in create category form </returns>
        [HttpPost]
        [Route("parent-candidates")]
        public async Task<IActionResult> ParentCandidates([FromQuery] string? category_id, string lang)
        {

            lang = lang.NormalizeLang();
            List<CategoryModel> categories;

            categories =  _context.Categories.
                Include(c=>c.Name.Translations).
                Include(c => c.ChildCategories).ThenInclude(c=>c.Name.Translations).
                Where(c=>c.ParentCategoryId == null).
                Flatten(c=>c.ChildCategories).ToList();

            if (category_id != null)
            {
                if (!int.TryParse(category_id, out int id)) return BadRequest("Check parameters!");

                var category = await _context.Categories.Include(c=>c.ChildCategories).FirstOrDefaultAsync(c=>c.Id == id);
                if (category == null) return NotFound("No such category!");

                foreach( var child in category.ChildCategories)
                {
                    categories.Remove(child);   
                }

                categories.Remove(category);

            }

            var result = new List<dynamic>();

            foreach (var category in categories)
            {
                result.Add(new
                {
                    id = category.Id,
                    name = category.Name.Content(lang),
                });
            }

            return new JsonResult(new { data = result });

        }


        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateCategory([FromForm] Category category)
        {
            
            if(category.ShowInCategoryId != null ) {

                if (!_context.Categories.Any(c => c.Id == category.ShowInCategoryId)) return BadRequest("Wrong additional category!");
            }

            var parent = await _context.Categories.FindAsync(category.ParentId);
            if (parent == null) return BadRequest("Parent category is bad!");

            TextContent? CategoryName = new TextContent();

            try
            {
                if (category.Names != null)
                {
                    Dictionary<string, string>? names = JsonConvert.DeserializeObject<Dictionary<string, string>>(category.Names);

                    if(names != null)
                    {
                        if (names.ContainsKey("UK"))
                        {
                            CategoryName = TextContentHelper.CreateTextContent(_context, names["UK"], "UK");
                            names.Remove("UK");
                            _context.textContents.Add(CategoryName);
                            _context.SaveChanges();
                        }
                        else
                            throw new Exception("No default UK localization found - check data!");

                        foreach (var name in names)
                        {
                            TextContentHelper.UpdateTextContent(_context, name.Value, CategoryName.Id, name.Key.ToUpper());
                        }
                    }
                }

            }
            catch (Exception e)
            {

                return BadRequest("Categories names value is malformed! : "+ e.Message);
            }

            var new_category = new CategoryModel()
            {
                Name = CategoryName,
                ParentCategory = parent,
                ImgData = category.RootIconFile == null ? null : category.RootIconFile.OpenReadStream().ToByteArray(),
                ImgUrl = _fileService.SaveFileFromStream(BucketNames.category.ToString(), category.CategoryImage.FileName, category.CategoryImage.OpenReadStream()).Result,
                ShowInCategoryId = category.ShowInCategoryId
            };

            return Ok(category);
        }
    }
}
