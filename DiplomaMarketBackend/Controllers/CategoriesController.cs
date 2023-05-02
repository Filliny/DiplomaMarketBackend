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

            var start = DateTime.Now;

            var stop_categories = await _context.Categories.AsNoTracking().
                Include(c=>c.Name.Translations).
                Include(c => c.ChildCategories).ThenInclude(cl=>cl.Name.Translations).
                Include(c=>c.ChildCategories).ThenInclude(child=>child.ChildCategories).
                ThenInclude(lc=>lc.Name.Translations)
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

                foreach(var mid_cat in category.ChildCategories)
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
                        BigPicture = Request.GetImageURL(BucketNames.category.ToString(), mid_cat.ImgUrl)
                    };

                    mid_cat.ChildCategories.AddRange(get_childs);

                    foreach(var low_cat in mid_cat.ChildCategories)
                    {
                        if (low_cat.is_active == false) continue;

                        var olow_cat = new Models.OutCategory()
                        {
                            Name = low_cat.Name.Content(lang),
                            Id = low_cat.Id,
                            SmallIcon = low_cat.ImgData != null ? Convert.ToBase64String(low_cat.ImgData) : null,
                            BigPicture = Request.GetImageURL(BucketNames.category.ToString(), low_cat.ImgUrl)
                        };

                        omid_cat.Children.Add(olow_cat);
                    }
                    out_cat.Children.Add(omid_cat);
                }

                new_result.Add(out_cat);
            }



            //var top_categories = _context.Categories
            //     .Include(c => c.Name)
            //     .ThenInclude(n => n.Translations)
            //     .Where(c => c.ParentCategoryId == null && c.is_active != false)
            //     .ToList();

            //var result = new List<Models.OutCategory>();

            //string loge = "\n\n\nCategory tree\n\n\n";
            //var baseUrl = Request.Scheme + "://" + Request.Host + "/api/Goods/category/";

            //foreach (var category in top_categories)
            //{
            //    var output_cat = new Models.OutCategory();

            //    if (category.Name != null)
            //    {
            //        output_cat.Name = category.Name.Content(lang);
            //        output_cat.Id = category.Id;
            //        output_cat.SmallIcon = category.ImgData != null ? Convert.ToBase64String(category.ImgData) : null;
            //        output_cat.BigPicture = category.ImgUrl != "" ? baseUrl + category.ImgUrl + ".jpg" : null;
            //        loge += "\n" + category.Name.Content(lang);
            //    }

            //    var mid_childs = _context.Categories.Include(c => c.Name).ThenInclude(n => n.Translations).Where(c => c.ParentCategoryId == category.Id && c.is_active != false).ToList();

            //    //categories adittionally showed in parent
            //    var additions = _context.Categories.Include(c => c.Name).ThenInclude(n => n.Translations).Where(c => c.ShowInCategoryId == category.Id && c.is_active != false).ToList();

            //    mid_childs.AddRange(additions);


            //    foreach (var child in mid_childs)
            //    {
            //        var output_mchild = new Models.OutCategory();
            //        if (child.Name != null)
            //        {
            //            output_mchild.Name = child.Name.Content(lang);
            //            output_mchild.Id = child.Id;
            //            output_mchild.SmallIcon = child.ImgData != null ? Convert.ToBase64String(child.ImgData) : null;
            //            output_mchild.BigPicture = child.ImgUrl != "" ? baseUrl + child.ImgUrl + ".jpg" : null;
            //            loge += "\n--" + child.Name.Content(lang);
            //        }

            //        var low_childs = _context.Categories.Include(c => c.Name).ThenInclude(n => n.Translations).Where(c => c.ParentCategoryId == child.Id && c.is_active != false).ToList();

            //        //categories adittionally showed in parent
            //        var additions_l = _context.Categories.Include(c => c.Name).ThenInclude(n => n.Translations).Where(c => c.ShowInCategoryId == child.Id && c.is_active != false).ToList();

            //        low_childs.AddRange(additions_l);

            //        foreach (var lowchild in low_childs)
            //        {
            //            var output_lchild = new Models.OutCategory();
            //            if (child.Name != null)
            //            {
            //                output_lchild.Name = lowchild.Name.Content(lang);
            //                output_lchild.Id = lowchild.Id;
            //                output_lchild.SmallIcon = lowchild.ImgData != null ? Convert.ToBase64String(lowchild.ImgData) : null;
            //                output_lchild.BigPicture = lowchild.ImgUrl != "" ? baseUrl + lowchild.ImgUrl + ".jpg" : null;
            //                loge += "\n----" + lowchild.Name.Content(lang);
            //            }

            //            output_mchild.Children.Add(output_lchild);
            //        }

            //        output_cat.Children.Add(output_mchild);
            //    }

            //    //_logger.LogInformation(loge);

            //    result.Add(output_cat);
            //}
            var end = DateTime.Now;

            _logger.LogInformation("Query time: "+(end - start).ToString());

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
        public async Task<IActionResult> GetSubCategories([FromQuery] string category_Id, string lang)
        {
            lang = lang.NormalizeLang();

            var subcategories = new List<dynamic>();
            var breadcrumbs = new List<dynamic>();

            if (int.TryParse(category_Id, out int id))
            {
                var output_cat = await _context.Categories.AsNoTracking().
                    Include(c => c.ChildCategories).ThenInclude(c => c.Name).ThenInclude(c => c.Translations).
                    Include(c => c.Name).ThenInclude(n => n.Translations).
                    Where(c => c.ParentCategoryId == id).ToListAsync();

                var add_cat = await _context.Categories.AsNoTracking().
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
        public async Task<IActionResult> GetFlatList([FromQuery] string? search, string lang, string limit, string page)
        {
            lang = lang.NormalizeLang();

            if (int.TryParse(limit, out int quantity) && int.TryParse(page, out int page_num))
            {
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


        /// <summary>
        /// Get one category data (for admin page)
        /// </summary>
        /// <param name="category_Id">Category id</param>
        /// <param name="lang">language</param>
        /// <returns>One category data</returns>
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

                    is_active = category.is_active,

                    names = new Dictionary<string, string>(),

                    children = new List<dynamic>()
                };

                foreach(var name in category.Name.Translations)
                     result.names.Add(name.LanguageId??"",name.TranslationString?? ""); 

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

        /// <summary>
        /// Create new category
        /// </summary>
        /// <param name="category">Category data from form</param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateCategory([FromForm] Category category)
        {
            
            if(category.ShowInCategoryId != null ) {

                if (!_context.Categories.Any(c => c.Id == category.ShowInCategoryId)) return BadRequest("Wrong additional category!");
            }

            if (category.ParentId == null && category.RootIconFile == null) return BadRequest("Icon file for root category not present!");

            var parent = await _context.Categories.FindAsync(category.ParentId);
            //if (parent == null) return BadRequest("Parent category is bad!");

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
                ImgUrl = category.CategoryImage == null?null: _fileService.SaveFileFromStream(BucketNames.category.ToString(), category.CategoryImage.FileName, category.CategoryImage.OpenReadStream()).Result,
                is_active = category.IsActive,
                ShowInCategoryId = category.ShowInCategoryId
            };

            _context.Categories.Add(new_category);
            _context.SaveChanges();

            return Ok(category);
        }

        /// <summary>
        /// Update category
        /// </summary>
        /// <param name="category">Category data from form</param>
        /// <returns>Inbound category</returns>
        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> UpdateCategory([FromForm] Category category)
        {
            var edit_category = _context.Categories.Include(c=>c.Name.Translations).FirstOrDefault(c=>c.Id == category.Id);

            if (edit_category == null) return NotFound("Category id is wrong!");

            if (category.ShowInCategoryId != null)
            {
                if (!_context.Categories.Any(c => c.Id == category.ShowInCategoryId)) return BadRequest("Wrong additional category!");
            }

            if (category.ParentId == null && category.RootIconFile == null) return BadRequest("Icon file for root category not present!");

            var parent = await _context.Categories.FindAsync(category.ParentId);

            var NameContent = edit_category.Name;

            try
            {
                if (category.Names != null)
                {
                    Dictionary<string, string>? names = JsonConvert.DeserializeObject<Dictionary<string, string>>(category.Names);

                    if (names != null)
                    {
                        if (names.ContainsKey("UK"))
                        {

                            if (!NameContent.OriginalText.Equals(names["UK"]))
                            {
  
                                NameContent.OriginalText = names["UK"];

                            }
                        }

                        foreach (var name in names)
                        {
                            TextContentHelper.UpdateTextContent(_context, name.Value, NameContent.Id, name.Key.ToUpper());
                        }

                    }
                }

                _context.textContents.Update(NameContent);
            }
            catch (Exception e)
            {

                return BadRequest("Categories names value is malformed! : " + e.Message);
            }

            edit_category.ParentCategory = parent;
            edit_category.ShowInCategoryId = category.ShowInCategoryId;
            edit_category.is_active = category.IsActive;
            edit_category.ImgData = category.RootIconFile == null ? null : category.RootIconFile.OpenReadStream().ToByteArray();

            if(category.CategoryImage != null && edit_category.ImgUrl != null)
            {
                _fileService.DeleteFile(BucketNames.category.ToString(), edit_category.ImgUrl);

            }

            edit_category.ImgUrl = category.CategoryImage == null ? null : _fileService.SaveFileFromStream(BucketNames.category.ToString(), category.CategoryImage.FileName, category.CategoryImage.OpenReadStream()).Result;

            _context.Categories.Update(edit_category);
            _context.SaveChanges();

            return Ok(category);
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
        public async Task<IActionResult> DeleteCategory([FromQuery] string category_id)
        {
            if(int.TryParse(category_id, out var id))
            {

                var category = await _context.Categories.FindAsync(id);
                if(category != null)
                {
                    if (_context.Categories.Any(c => c.ParentCategoryId == id) && _context.Articles.Any(a => a.CategoryId == id))
                        return StatusCode(StatusCodes.Status500InternalServerError, 
                            new
                            {
                                Status = "Error",
                                Message = "Category is not empty or have child categories",
                                child_count = _context.Categories.Count(a=>a.ParentCategoryId == id),
                                articles_related = _context.Articles.Count(a=>a.CategoryId == id)
                            });

                    _context.Categories.Remove(category);
                    _context.SaveChanges();

                    return Ok();

                }

                return NotFound("No such category!");
            }

            return BadRequest("Check parameters!");
        }
    }

}
