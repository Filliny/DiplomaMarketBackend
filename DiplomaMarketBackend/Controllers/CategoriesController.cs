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
        public async Task<IActionResult> GetFullTree([FromQuery] string lang)
        {
            lang = lang.NormalizeLang();

            var stop_categories = await _context.Categories.AsNoTracking().
                Include(c => c.Name.Translations).
                Include(c => c.ChildCategories).ThenInclude(cl => cl.Name.Translations).
                Include(c => c.ChildCategories).ThenInclude(child => child.ChildCategories).
                ThenInclude(lc => lc.Name.Translations)
                .Where(c => c.ParentCategoryId == null)
                .ToListAsync();

            var new_result = new List<Models.OutCategory>();

            foreach (var category in stop_categories)
            {
                if (category.is_active == false) continue;

                var get_childs = await _context.Categories.AsNoTracking().
                    Include(c => c.Name.Translations).
                    Include(c => c.ChildCategories).ThenInclude(c => c.Name.Translations).
                    Where(c => c.ShowInCategoryId == category.Id).ToListAsync();

                var out_cat = new Models.OutCategory()
                {
                    Name = category.Name.Content(lang),
                    Id = category.Id,
                    SmallIcon = category.ImgData != null ? Convert.ToBase64String(category.ImgData) : null,
                    BigPicture = Request.GetImageURL(BucketNames.category.ToString(), category.ImgUrl)

                };

                //adding show_in
                category.ChildCategories.AddRange(get_childs);

                foreach (var mid_cat in category.ChildCategories)
                {
                    if (mid_cat.is_active == false) continue;

                    var get_mchilds = await _context.Categories.AsNoTracking().
                    Include(c => c.Name.Translations).
                    Include(c => c.ChildCategories).ThenInclude(c => c.Name.Translations).
                    Where(c => c.ShowInCategoryId == mid_cat.Id).ToListAsync();

                    var omid_cat = new Models.OutCategory()
                    {
                        Name = mid_cat.Name.Content(lang),
                        Id = mid_cat.Id,
                        SmallIcon = mid_cat.ImgData != null ? Convert.ToBase64String(mid_cat.ImgData) : null,
                        BigPicture = Request.GetImageURL(BucketNames.category.ToString(), mid_cat.ImgUrl),
                        ParentId = category.Id,

                    };

                    mid_cat.ChildCategories.AddRange(get_childs);

                    foreach (var low_cat in mid_cat.ChildCategories)
                    {
                        if (low_cat.is_active == false) continue;

                        var olow_cat = new Models.OutCategory()
                        {
                            Name = low_cat.Name.Content(lang),
                            Id = low_cat.Id,
                            SmallIcon = low_cat.ImgData != null ? Convert.ToBase64String(low_cat.ImgData) : null,
                            BigPicture = Request.GetImageURL(BucketNames.category.ToString(), low_cat.ImgUrl),
                            ParentId = mid_cat.Id,

                        };

                        omid_cat.Children.Add(olow_cat);
                    }
                    out_cat.Children.Add(omid_cat);
                }

                new_result.Add(out_cat);
            }

            return new JsonResult(new { data = new_result });

        }


        /// <summary>
        /// Get sub-categories of given top category
        /// </summary>
        /// <param name="category_Id">Top category id</param>
        /// <param name="lang">language</param>
        /// <returns>List of sub-categories with images and inner categories list, also include breadcrumbs to top category</returns>
        [HttpGet]
        [Route("get-sub")]
        public async Task<IActionResult> GetSubCategories([FromQuery] int category_Id, string lang)
        {
            lang = lang.NormalizeLang();

            var subcategories = new List<dynamic>();
            var breadcrumbs = new List<dynamic>();

            var output_cat = await _context.Categories.AsNoTracking().
                Include(c => c.ChildCategories).ThenInclude(c => c.Name).ThenInclude(c => c.Translations).
                Include(c => c.Name).ThenInclude(n => n.Translations).
                Where(c => c.ParentCategoryId == category_Id).ToListAsync();

            var add_cat = await _context.Categories.AsNoTracking().
                Include(c => c.ChildCategories).ThenInclude(c => c.Name).ThenInclude(c => c.Translations).
                Include(c => c.Name).ThenInclude(n => n.Translations).
                Where(c => c.ShowInCategoryId == category_Id).ToListAsync();

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

            int? curr_id = category_Id;

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

            return new JsonResult(new { data = subcategories, breadcrumbs });

        }

        /// <summary>
        /// Get all categories list in plain format (for admin page)
        /// </summary>
        /// <param name="search">search string (can be null)</param>
        /// <param name="lang">language</param>
        /// <param name="limit">limit results on page</param>
        /// <param name="page">page number</param>
        /// <returns>List of all categories or filtered list</returns>
        [HttpGet]
        [Route("flat")]
        public async Task<IActionResult> GetFlatList([FromQuery] string? search, string lang, int limit, int page)
        {
            lang = lang.NormalizeLang();

            List<CategoryModel> categories;

            if (search != null)
            {
                categories = await _context.Categories.Include(c => c.Name.Translations).Where(c => c.Name.Translations.Any(t => t.TranslationString.ToLower().Contains(search.ToLower()) && t.LanguageId == lang)).ToListAsync();
            }
            else
            {
                categories = await _context.Categories.Include(c => c.Name.Translations).ToListAsync();
            }

            var found = categories.Count();

            int total_pages = (int)Math.Ceiling((decimal)found / (decimal)limit);

            if (page == 0) page = 1;
            if (page > total_pages) page = total_pages;

            int skip = (page - 1) * limit;

            var found_cat = categories.Skip(skip).Take(limit).ToList();


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
                current_page = page
            };

            return new JsonResult(response);


        }


        /// <summary>
        /// Get one category data (for admin page)
        /// </summary>
        /// <param name="category_Id">Category id</param>
        /// <param name="lang">language</param>
        /// <returns>One category data</returns>
        /// <response code="404">If category not found</response>
        [HttpGet]
        [Route("category")]
        public async Task<ActionResult<Category>> GetOneCategory([FromQuery] int category_Id, string lang)
        {
            lang = lang.NormalizeLang();

            var category = await _context.Categories.Include(c => c.ParentCategory.Name.Translations).Include(c => c.Name.Translations).Include(c => c.ChildCategories).ThenInclude(c => c.Name.Translations).FirstOrDefaultAsync(c => c.Id == category_Id);

            if (category == null) return NotFound(new Result
            {
                Status = "Error",
                Message = "Category not found!"
            });

            var result = new Category
            {
                id = category_Id,
                existing_image = category.ImgUrl == null ? null : Request.GetImageURL(BucketNames.category, category.ImgUrl),
                existing_icon = category.ImgData != null ? Convert.ToBase64String(category.ImgData) : null,
                showin_category_id = category.ShowInCategoryId,
                parent_id = category.ParentCategoryId,
                is_active = category.is_active,
                names = JsonConvert.SerializeObject(category.Name.Translations.ToDictionary(t => t.LanguageId ?? "", t => t.TranslationString ?? "")),
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

            return new JsonResult(result);


        }

        /// <summary>
        /// Parent candidates categories
        /// </summary>
        /// <param name="category_id">category id for move category (excludes from result as well as childs) can be null</param>
        /// <param name="lang">language</param>
        /// <returns>Return list of categories as candidates for parent categories in create category form </returns>
        /// <response code="404">If category not found</response>
        [HttpGet]
        [Route("parent-candidates")]
        public async Task<IActionResult> ParentCandidates([FromQuery] int? category_id, string lang)
        {

            lang = lang.NormalizeLang();
            List<CategoryModel> categories;

            categories = _context.Categories.
                Include(c => c.Name.Translations).
                Include(c => c.ChildCategories).ThenInclude(c => c.Name.Translations).
                Where(c => c.ParentCategoryId == null).
                Flatten(c => c.ChildCategories).ToList();

            if (category_id != null)
            {
                var category = await _context.Categories.Include(c => c.ChildCategories).FirstOrDefaultAsync(c => c.Id == category_id);
                if (category == null) return NotFound(new Result
                {
                    Status = "Error",
                    Message = "Category not found!"
                });

                foreach (var child in category.ChildCategories)
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

        /// <summary>
        /// Create new category
        /// </summary>
        /// <param name="category">Category data from form</param>
        /// <returns>Ok if success with incoming entity and new id</returns>
        /// <response code="400">If any data error</response>
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateCategory([FromForm] Category category)
        {

            if (category.showin_category_id != null)
            {

                if (!_context.Categories.Any(c => c.Id == category.showin_category_id)) return BadRequest(new Result
                {
                    Status = "Error",
                    Message = "Wrong additional category!",
                    Entity = category
                });
            }

            if (category.parent_id == null && category.root_icon == null) return BadRequest(new Result
            {
                Status = "Error",
                Message = "Icon file for root category not present!",
                Entity = category
            });

            var parent = await _context.Categories.FindAsync(category.parent_id);
            //if (parent == null) return BadRequest("Parent category is bad!");

            TextContent? CategoryName = new TextContent();

            try
            {
                if (category.names != null)
                {
                    Dictionary<string, string>? names = JsonConvert.DeserializeObject<Dictionary<string, string>>(category.names);

                    if (names == null)
                        throw new Exception("Names parsing error!");

                    CategoryName = TextContentHelper.CreateFromDictionary(_context, names, false);
                }

            }
            catch (Exception e)
            {

                return BadRequest(new Result
                {
                    Status = "Error",
                    Message = "Categories names value is malformed! : " + e.Message,
                    Entity = category,
                });
            }

            var new_category = new CategoryModel()
            {
                Name = CategoryName,
                ParentCategory = parent,
                ImgData = category.root_icon == null ? null : category.root_icon.OpenReadStream().ToByteArray(),
                ImgUrl = category.category_image == null ? null : _fileService.SaveFileFromStream(BucketNames.category.ToString(), category.category_image.FileName, category.category_image.OpenReadStream()).Result,
                is_active = category.is_active,
                ShowInCategoryId = category.showin_category_id
            };

            _context.Categories.Add(new_category);
            _context.SaveChanges();
            category.id = new_category.Id;

            return Ok(new Result
            {
                Status="Success",
                Message="Category successfully created!",
                Entity = category
            });
        }

        /// <summary>
        /// Update category
        /// </summary>
        /// <param name="category">Category data from form</param>
        /// <returns>Inbound category</returns>
        /// <response code="400">If any data error</response>
        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> UpdateCategory([FromForm] Category category)
        {
            var edit_category = _context.Categories.Include(c => c.Name.Translations).FirstOrDefault(c => c.Id == category.id);

            if (edit_category == null) return NotFound(new Result
            {
                Status = "Error",
                Message = "Category id is wrong!",
                Entity = category
            });

            if (category.showin_category_id != null)
            {
                if (!_context.Categories.Any(c => c.Id == category.showin_category_id)) return BadRequest(new Result
                {
                    Status = "Error",
                    Message = "Wrong additional category!",
                    Entity = category
                });
            }

            if (category.parent_id == null && category.root_icon == null) return BadRequest(new Result
            {
                Status = "Error",
                Message = "Icon file for root category not present!",
                Entity = category
            });

            var parent = await _context.Categories.FindAsync(category.parent_id);
            //var NameContent = edit_category.Name;

            try
            {
                if (category.names != null)
                {
                    Dictionary<string, string>? names = JsonConvert.DeserializeObject<Dictionary<string, string>>(category.names);

                    if (names == null)
                        throw new Exception("Names parsing error!");

                    TextContentHelper.UpdateFromDictionary(_context, edit_category.Name, names,false);
                }

                _context.textContents.Update(edit_category.Name);
            }
            catch (Exception e)
            {
                return BadRequest(new Result
                {
                    Status="Error",
                    Message = "Categories names value is malformed! : " + e.Message,
                    Entity = category
                });
            }

            edit_category.ParentCategory = parent;
            edit_category.ShowInCategoryId = category.showin_category_id;
            edit_category.is_active = category.is_active;
            edit_category.ImgData = category.root_icon == null ? null : category.root_icon.OpenReadStream().ToByteArray();

            if (category.category_image != null && edit_category.ImgUrl != null)
            {
                _fileService.DeleteFile(BucketNames.category.ToString(), edit_category.ImgUrl);

            }

            edit_category.ImgUrl = category.category_image == null ? null : _fileService.SaveFileFromStream(BucketNames.category.ToString(), category.category_image.FileName, category.category_image.OpenReadStream()).Result;

            _context.Categories.Update(edit_category);
            _context.SaveChanges();

            return Ok(new Result
            {
                Status="Success",
                Message= "Category updated",
                Entity = category
            });
        }

        /// <summary>
        /// Delete category by id
        /// </summary>
        /// <param name="category_id"></param>
        /// <returns>Nothing if delete success</returns>
        /// <response code="500">If the request value is bad or category is not empty/have childs</response>
        /// <response code="404">If the category is not found</response>
        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteCategory([FromQuery] int category_id)
        {
            var category = await _context.Categories.Include(c => c.Name.Translations).FirstOrDefaultAsync(c => c.Id == category_id);

            if (category != null)
            {
                if (_context.Categories.Any(c => c.ParentCategoryId == category_id) && _context.Articles.Any(a => a.CategoryId == category_id))
                    return base.StatusCode(StatusCodes.Status500InternalServerError,
                        new
                        {
                            Status = "Error",
                            Message = "Category is not empty or have child categories",
                            Entity = new
                            {
                                child_count = _context.Categories.Count(a => a.ParentCategoryId == category_id),
                                articles_related = _context.Articles.Count(a => a.CategoryId == category_id)
                            }

                        });

                _context.textContents.Remove(category.Name);
                TextContentHelper.Delete(_context, category.Name);

                _context.Categories.Remove(category);
                _context.SaveChanges();

                return Ok();

            }

            return NotFound(new Result
                    {
                        Status = "Error",
                        Message = "Category not found",
                    });

        }
    }

}
