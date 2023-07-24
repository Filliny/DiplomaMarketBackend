using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Models;
using DiplomaMarketBackend.Helpers;
using DiplomaMarketBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;

namespace DiplomaMarketBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoodsController : Controller
    {
        ILogger<GoodsController> _logger;
        BaseContext _context;
        IFileService _fileService;

        public GoodsController(ILogger<GoodsController> logger, BaseContext context, IFileService fileService)
        {
            _logger = logger;
            _context = context;
            _fileService = fileService;
        }

        /// <summary>
        /// Get all articles ids from given category
        /// </summary>
        /// <param name="lang">language (e.g. uk\ru)</param>
        /// <param name="category_Id">id of category</param>
        /// <returns>Json result with list of ids on key "ids"</returns>
        /// <response code="400">If the request values is bad</response>
        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> GetCategoryArticles([FromQuery] string lang, int category_Id)
        {
            int goods_limit = 60;
            
            var category = await _context.Categories.Include(c => c.ChildCategories).
                ThenInclude(c => c.ChildCategories).
                FirstOrDefaultAsync(c => c.Id == category_Id);

            if (category == null)
            {
                return NotFound("No such category");
            }

            var all_cat_list = category.ChildCategories.Flatten(c => c.ChildCategories).ToList();
            all_cat_list.Add(category);
            all_cat_list.AddRange(_context.Categories.Where(c => c.ShowInCategoryId == category_Id).ToList());

            var all_articles = _context.Articles.Where(a => all_cat_list.Contains(a.Category)).Select(a => a.Id).ToList();

            dynamic dresponse = new ExpandoObject();
            IDictionary<string, object> response = (IDictionary<string, object>)dresponse;

            response.Add("ids", all_articles.Skip(0).Take(goods_limit));
            response.Add("active_pages", new int[] { 1 });
            response.Add("goods_in_category", all_articles.Count);
            response.Add("total_pages", Math.Ceiling((double)all_articles.Count / goods_limit));
            response.Add("goods_limit", goods_limit);
            response.Add("shown_page", 1);

            return new JsonResult(new { data = response });

        }


        /// <summary>
        /// Get articles for sub-categories with single-page pagination
        /// </summary>
        /// <param name="category_Id">Categpry id</param>
        /// <param name="goods_on_page">goods count to display</param>
        /// <param name="page">page to display starts from 1</param>
        /// <param name="lang">language</param>
        /// <returns>List of articles taken in any action sell</returns>
        /// <response code="400">If the request value is bad</response>
        [HttpGet]
        [Route("category-articles")]
        [ResponseCache(VaryByQueryKeys = new[] { "category_Id", "lang", "goods_on_page", "page" }, VaryByHeader = "User-Agent", Duration = 3600)]
        public async Task<IActionResult> GetCategoryArticlesPages([FromQuery] int category_Id,  int page, string lang ,int goods_on_page=10)
        {
            lang = lang.NormalizeLang();

            var category = await _context.Categories.Include(c => c.ChildCategories).ThenInclude(c => c.ChildCategories).FirstOrDefaultAsync(c => c.Id == category_Id);

            if (category == null)
            {
                return NotFound("No such category");
            }

            var flat = category.ChildCategories.Flatten(c => c.ChildCategories).ToList();
            flat.Add(category);
            flat.AddRange(_context.Categories.Where(c => c.ShowInCategoryId == category_Id).ToList());
            
            if (page == 0) page = 1;

            var articles = new List<dynamic>();
            
            var actionGoodsQ =  _context.Articles.
                Include(a => a.Title).ThenInclude(t => t.Translations).
                Include(a => a.Category).
                Where(a => flat.Contains(a.Category));
            
            int total_goods = actionGoodsQ.Count();
            int total_pages = (int)Math.Ceiling((decimal)total_goods / (decimal)goods_on_page);

            if (page == 0) page = 1;
            if (page > total_pages) page = total_pages;

            int skip = (page - 1) * goods_on_page;

            var action_goods = await actionGoodsQ.Skip(skip).Take(goods_on_page).ToListAsync();
            
            var filterCounts = await actionGoodsQ.
                Include(a => a.CharacteristicValues).
                SelectMany(a=>a.CharacteristicValues.Select(v=>new {v,a})).
                GroupBy(x=>x.v.Id).
                Select(g=> new
                {
                    key=g.Key,
                    count = g.Select(s=>s.a).Count()
                })
                .ToListAsync();

            foreach (var article in action_goods)
            {
                articles.Add(ArticleToDtoLight(article, lang));
            }

            var result = new
            {
                data = new
                {
                    articles,
                    total_goods,
                    total_pages,
                    displayed_page = page,
                    filter_data = filterCounts
                }
            };

            return new JsonResult(result);

        }

        /// <summary>
        /// Get articles for sub-categories with single-page pagination
        /// </summary>
        /// <param name="category_Id">Category id</param>
        /// <param name="goods_on_page">goods count to display</param>
        /// <param name="page">page to display starts from 1</param>
        /// <param name="lang">language</param>
        /// <param name="filter">Filter entity with filtering parameters</param>
        /// <returns>List of articles taken in any action sell</returns>
        /// <response code="400">If the request value is bad</response>
        [HttpPost]
        [Route("category-articles")]
        [ResponseCache(VaryByQueryKeys = new[] { "category_Id", "lang", "goods_on_page", "page" }, VaryByHeader = "User-Agent", Duration = 3600)]
        public async Task<IActionResult> GetCategoryArticlesPages([FromQuery] int category_Id, int goods_on_page, int page, string lang,
            [FromBody] Filter? filter)
        {
            
            lang = lang.NormalizeLang();

            var category = await _context.Categories.Include(c => c.ChildCategories).ThenInclude(c => c.ChildCategories).FirstOrDefaultAsync(c => c.Id == category_Id);

            if (category == null)
            {
                return NotFound("No such category");
            }

            //to include shown in category
            var flat = category.ChildCategories.Flatten(c => c.ChildCategories).ToList();
            flat.Add(category);
            flat.AddRange(_context.Categories.Where(c => c.ShowInCategoryId == category_Id).ToList());
            
            if (page == 0) page = 0;

            var articles = new List<dynamic>();

            var actionGoodsQ = await _context.Articles.
                Include(a => a.Title).ThenInclude(t => t.Translations).
                Include(a => a.Category).
                Include(a => a.CharacteristicValues).
                Where(a => flat.Contains(a.Category)).ToListAsync();
            
                      
            //filter section
            if (filter != null)
            {
                //parsing filter diapason list to real values ids
                if (filter.slider_values != null && filter.slider_values.Count() > 0)
                {
                    var add_search_ids = new List<int>();

                    foreach (var value in filter.slider_values)
                    {
                        var select_list = _context.CharacteristicValues.Include(v => v.Title).
                            Where(v => v.CharacteristicTypeId == value.characteristic_id).ToList();

                        foreach (var select in select_list)
                        {
                            if (int.TryParse(select.Title.OriginalText, out int val))
                            {

                                if (val >= value.lower_value && val <= value.upper_value) add_search_ids.Add(select.Id);
                            }
                        }
                    }

                    filter.values_id.AddRange(add_search_ids);
                }

                if (filter.brands_id.Any())
                    actionGoodsQ = actionGoodsQ.Where(a => filter.brands_id.Contains(a.BrandId)).ToList();
                
                if (filter.values_id.Any())
                    actionGoodsQ = actionGoodsQ.Where(a => filter.values_id.All(f=>a.CharacteristicValues.Any(c=>c.Id == f))).ToList();

                if (filter.price_low != 0)
                    actionGoodsQ = actionGoodsQ.Where(a => a.Price >= filter.price_low).ToList();

                if (filter.price_high != 0)
                    actionGoodsQ = actionGoodsQ.Where(a => a.Price <= filter.price_high).ToList();
                
            }
            
            int total_goods = actionGoodsQ.Count();
            int total_pages = (int)Math.Ceiling((decimal)total_goods / (decimal)goods_on_page);

            if (page == 0) page = 1;
            if (page > total_pages) page = total_pages;

            int skip = (page - 1) * goods_on_page;

            var action_goods =  actionGoodsQ.Skip(skip).Take(goods_on_page);
            
            
            var filterCounts =  actionGoodsQ.
               
                SelectMany(a=>a.CharacteristicValues.Select(v=>new {v,a})).
                GroupBy(x=>x.v.Id).
                Select(g=> new
                {
                    key=g.Key,
                    count = g.Select(s=>s.a).Count()
                })
                .ToList();

            foreach (var article in action_goods)
            {

                articles.Add(ArticleToDtoLight(article, lang));

            }

            var result = new
            {
                data = new
                {
                    articles,
                    total_goods,
                    total_pages,
                    displayed_page = page,
                    filter_data = filterCounts
                }
            };

            return new JsonResult(result);

        }


        /// <summary>
        /// Get articles for sub-categories with multi-page pagination for dynamic loading
        /// </summary>
        /// <param name="category_Id">Id of subcategory</param>
        /// <param name="goods_on_page">goods count to display</param>
        /// <param name="lang">language</param>
        /// <param name="pages">Array of pages numbers to display (starts from 1)</param>
        /// <returns></returns>
        [HttpPost]
        [Route("category-articles-paged")]
        public async Task<IActionResult> GetCategoryArticlesPagination([FromQuery] int category_Id, string lang, int goods_on_page, [FromBody] int[] pages)
        {
            lang = lang.NormalizeLang();
            if (goods_on_page == 0) goods_on_page = 10;

            var category = await _context.Categories.
                Include(c => c.ChildCategories).
                ThenInclude(c => c.ChildCategories).
                FirstOrDefaultAsync(c => c.Id == category_Id);

            if (category == null)
            {
                return NotFound("No such category");
            }

            var all_cat_list = category.ChildCategories.
                SelectMany(x => x.ChildCategories).
                Concat(category.ChildCategories.SelectMany(x => x.ChildCategories).
                    SelectMany(x => x.ChildCategories)).ToList();
            
            all_cat_list.Add(category);
            
            var articles = new List<dynamic>();

            var category_goods = await _context.Articles.
                Include(a => a.Title).ThenInclude(t => t.Translations).
                Include(a => a.Category).
                Where(a => all_cat_list.Contains(a.Category)).ToListAsync();
            
            int total_goods = category_goods.Count;
            int total_pages = (int)Math.Ceiling((decimal)total_goods / (decimal)goods_on_page);

            var pages_arr = new List<int>();
            foreach (int page in pages)
            {

                if (page > total_pages) continue;
                if (page == 0) continue;

                pages_arr.Add(page);
            }

            pages_arr.Sort();

            var paged_goods = new List<ArticleModel>();

            foreach (int page in pages_arr)
            {
                int skip = (page) * goods_on_page;
                paged_goods.AddRange(category_goods.Skip(skip).Take(goods_on_page).ToList());
            }

            foreach (var article in paged_goods)
            {
                articles.Add(ArticleToDtoLight(article, lang));
            }

            var result = new
            {
                data = new
                {
                    articles,
                    total_goods,
                    total_pages,
                    displayed_pages = pages_arr
                }
            };

            return new JsonResult(result);

        }


        /// <summary>
        /// Get main article main parameters
        /// </summary>
        /// <param name="lang">language (e.g. uk\ru)</param>
        /// <param name="goodsId">id of article</param>
        /// <returns>main article parameters</returns>
        /// <response code="404">If the item is not exist</response>
        /// <response code="400">If the request value is bad</response>
        [HttpGet]
        [Route("get-main")]
        public async Task<IActionResult> GetArticleFull([FromQuery] string lang, int goodsId)
        {
            lang = lang.NormalizeLang();

            dynamic dresponse = new ExpandoObject();
            IDictionary<string, object> response = (IDictionary<string, object>)dresponse;
            
            var article = await _context.Articles.
                Include(a => a.Breadcrumbs).ThenInclude(b => b.Title).ThenInclude(t => t.Translations).
                Include(a => a.Title).ThenInclude(t => t.Translations).
                Include(a => a.Description).ThenInclude(t => t.Translations).
                Include(a => a.Docket).ThenInclude(t => t.Translations).
                Include(a => a.Brand).
                Include(a => a.Seller).
                Include(a => a.Warning).ThenInclude(w => w.Message).ThenInclude(m => m.Translations).
                Include(a => a.Actions).ThenInclude(a => a.Name).ThenInclude(n => n.Translations).
                Include(a => a.Video).
                FirstOrDefaultAsync(a => a.Id == goodsId);

            if (article == null)
            {
                return NotFound($"Article id={goodsId} not exist!");
            }

            response = ArticleToDto(article, lang);
            

            return new JsonResult(new { data = response });
        }

        /// <summary>
        /// Get article detailed characteristics by given id
        /// </summary>
        /// <param name="lang">language (e.g. uk\ru)</param>
        /// <param name="goodsId">id of article</param>
        /// <returns>all article characteristics</returns>
        /// <response code="404">If the item is not exist</response>
        /// <response code="400">If the request value is bad</response>
        [HttpGet]
        [Route("get-characteristic")]
        [ResponseCache(VaryByQueryKeys = new[] { "lang", "goodsId" }, Duration = 3600)]
        public async Task<IActionResult> GetArticleCharacteristics([FromQuery] string lang, string goodsId)
        {
            lang = lang.NormalizeLang();

            var characteristic_group_list = new List<dynamic>();


            if (int.TryParse(goodsId, out int art_id))
            {
                var article = await _context.Articles.
                    Include(a => a.CharacteristicValues).ThenInclude(v => v.Title.Translations).
                    Include(a => a.CharacteristicValues).ThenInclude(c => c.CharacteristicType.Group.groupTitle.Translations).
                    Include(a => a.CharacteristicValues).ThenInclude(c => c.CharacteristicType.Name.Translations).
                    Include(a => a.CharacteristicValues).ThenInclude(c => c.CharacteristicType.Title.Translations).
                    FirstOrDefaultAsync(a => a.Id == art_id);

                if (article == null) return NotFound("No such article!");

                var groups = article.CharacteristicValues.GroupBy(v => v.CharacteristicType).GroupBy(v => v.Key.Group).ToList();


                foreach (var group in groups)
                {
                    if (group.Key == null)
                    {
                        dynamic zero_group = new ExpandoObject();
                        IDictionary<string, object> dictionary_zero_group = (IDictionary<string, object>)zero_group;

                        dictionary_zero_group.Add("group_id", 0);
                        dictionary_zero_group.Add("group_order", 99999999);
                        dictionary_zero_group.Add("groupTitle", "");

                        var zero_group_options = new List<dynamic>();

                        foreach (var charakterystyc_type in group)
                        {
                            //skip fixed filter charakterystic
                            if (charakterystyc_type.Key.CategoryId == null) continue;

                            var list_values = new List<dynamic>();

                            foreach (var val in charakterystyc_type)
                            {
                                var to_list = new
                                {
                                    title = val.Title.Content(lang),
                                    href = val.href
                                };

                                list_values.Add(to_list);
                            }


                            var dyna_charakter = new
                            {
                                id = charakterystyc_type.Key.Id,
                                type = charakterystyc_type.Key.Type,
                                name = charakterystyc_type.Key.Name.Content(lang),
                                title = charakterystyc_type.Key.Title.Content(lang),
                                category_id = charakterystyc_type.Key.CategoryId,
                                status = charakterystyc_type.Key.Status,
                                order = charakterystyc_type.Key.Order,
                                comparable = charakterystyc_type.Key.Comparable,
                                values = list_values

                            };

                            zero_group_options.Add(dyna_charakter);
                        }

                        dictionary_zero_group.Add("options", zero_group_options);
                        characteristic_group_list.Add(zero_group);
                    }
                    else //for exist group key
                    {
                        dynamic d_group = new ExpandoObject();
                        IDictionary<string, object> dictionary_group = (IDictionary<string, object>)d_group;

                        dictionary_group.Add("group_id", group.Key.Id);
                        dictionary_group.Add("group_order", group.Key.group_order);
                        dictionary_group.Add("groupTitle", group.Key.groupTitle.Content(lang));

                        var group_options = new List<dynamic>();

                        foreach (var charakterystyc_type in group)
                        {
                            var list_values = new List<dynamic>();

                            foreach (var val in charakterystyc_type)
                            {
                                var to_list = new
                                {
                                    title = val.Title.Content(lang),
                                    href = val.href
                                };

                                list_values.Add(to_list);
                            }


                            var dyna_charakter = new
                            {
                                id = charakterystyc_type.Key.Id,
                                type = charakterystyc_type.Key.Type,
                                name = charakterystyc_type.Key.Name.Content(lang),
                                title = charakterystyc_type.Key.Title.Content(lang),
                                category_id = charakterystyc_type.Key.CategoryId,
                                status = charakterystyc_type.Key.Status,
                                order = charakterystyc_type.Key.Order,
                                comparable = charakterystyc_type.Key.Comparable,
                                values = list_values

                            };

                            group_options.Add(dyna_charakter);
                        }

                        dictionary_group.Add("options", group_options);
                        characteristic_group_list.Add(d_group);

                    }

                }

            }
            else
            {
                return BadRequest("Check request parameters!");
            }

            return new JsonResult(new { data = characteristic_group_list });
        }

        /// <summary>
        /// Get articles for actions 
        /// </summary>
        /// <param name="goods_on_page">goods count to display</param>
        /// <param name="page">page to display starts from 1</param>
        /// <param name="lang">language</param>
        /// <returns>List of articles taken in any action sell</returns>
        /// <response code="400">If the request value is bad</response>
        [HttpGet]
        [Route("all-actions")]
        [ResponseCache(VaryByQueryKeys = new[] { "lang", "goods_on_page", "page" }, VaryByHeader = "User-Agent", Duration = 360)]
        public async Task<IActionResult> GetAllActions([FromQuery] string lang, int goods_on_page=10 ,int page=1)
        {
            lang = lang.NormalizeLang();

            var articles = new List<dynamic>();
            
            var action_goods = await _context.Articles.
                Include(a => a.Title.Translations).
                Include(a => a.Description).ThenInclude(t => t.Translations).
                Include(a => a.Category).
                Include(a => a.Actions).ThenInclude(a => a.Name.Translations).
                Where(a => a.Actions.Count > 0).ToListAsync();


            int total_goods = action_goods.Count;
            int total_pages = (int)Math.Ceiling((decimal)total_goods / (decimal)goods_on_page);

            if (page > total_pages) page = total_pages;

            int skip = (page - 1) * goods_on_page;

            action_goods = action_goods.Skip(skip).Take(goods_on_page).ToList();

            foreach (var article in action_goods)
            {

                articles.Add(ArticleToDtoLight(article, lang));

            }

            var result = new
            {
                data = new
                {
                    articles,
                    total_goods,
                    total_pages,
                    displayed_page = page,
                    breadcrumbs = new List<dynamic> { new { id = 0, name = lang == "UK" ? "Акційні пропозиції" : "Акционные предложения" } }
                }
            };

            return new JsonResult(result);

        }


        /// <summary>
        /// Get articles for bonus points
        /// </summary>
        /// <param name="goods_on_page">goods count to display</param>
        /// <param name="page">page to display starts from 1</param>
        /// <param name="lang">language</param>
        /// <returns>List of articles sorted by bonus points</returns>
        /// <response code="400">If the request value is bad</response>
        [HttpGet]
        [Route("best-points")]
        [ResponseCache(VaryByQueryKeys = new[] { "lang", "goods_on_page", "page" }, VaryByHeader = "User-Agent", Duration = 360)]
        public async Task<IActionResult> GetBestPoints([FromQuery] string lang,int goods_on_page=10 ,int page=1)
        {
            lang = lang.NormalizeLang();

            var articles = new List<dynamic>();


            var action_goods = await  _context.Articles.
                Include(a => a.Title.Translations).
                Include(a => a.Description).ThenInclude(t => t.Translations).
                Include(a => a.Category).
                Include(a => a.Actions).ThenInclude(a => a.Name.Translations).
                Where(a => a.Points > 0).OrderByDescending(a=>a.Points).ToListAsync();


            int total_goods = action_goods.Count();
            int total_pages = (int)Math.Ceiling(total_goods / (decimal)goods_on_page);
            
            if (page > total_pages) page = total_pages;

            int skip = (page - 1) * goods_on_page;

            action_goods = action_goods.Skip(skip).Take(goods_on_page).ToList();

            foreach (var article in action_goods)
            {
                articles.Add(ArticleToDtoLight(article, lang));

            }

            var result = new
            {
                data = new
                {
                    articles,
                    total_goods,
                    total_pages,
                    displayed_page = page,
                    breadcrumbs = new List<dynamic> { new { id = 0, name = lang == "UK" ? "Товари з найвигіднішими балами" : "Товары с самыми выгодными баллами" } }
                }
            };

            return new JsonResult(result);
        }



        /// <summary>
        /// Gets file by url from Mongo
        /// </summary>
        /// <param name="bucket">Bucket name</param>
        /// <param name="id">picture id</param>
        /// <returns>file to display on page</returns>
        [HttpGet]
        [Route("{bucket}/{id}")]
        public async Task<IActionResult> bigImage(string bucket, string id)
        {

            string MimeType = "image/jpg";
            var img_id = id.Split('.')[0];

            var bytes = _fileService.GetFile(img_id, bucket).Result;
            return File(bytes, MimeType);
        }


        /// <summary>
        /// Gets hot new articles
        /// </summary>
        /// <param name="goods_on_page">goods count to display</param>
        /// <param name="page">page to display starts from 1</param>
        /// <param name="lang">language</param>
        /// <returns>List of latest added articles</returns>
        /// <response code="400">If the request value is bad</response>
        [HttpGet]
        [Route("hot-news")]
        [ResponseCache(VaryByQueryKeys = new[] { "lang", "goods_on_page", "page" }, VaryByHeader = "User-Agent", Duration = 60)]
        public async Task<IActionResult> GetHotNews([FromQuery] string goods_on_page, string page, string lang)
        {
            lang = lang.NormalizeLang();

            if (string.IsNullOrEmpty(page)) page = "1";

            var articles = new List<dynamic>();

            if (goods_on_page != null && int.TryParse(goods_on_page, out int quantity) && int.TryParse(page, out int page_num))
            {
                var rand = new Random();

                var action_goods = await _context.Articles.
                    Include(a => a.Title).ThenInclude(t => t.Translations).
                    Include(a => a.Category).
                    OrderByDescending(a => a.Id).Skip(rand.Next(1000)).Take(100).ToListAsync();
                
                int total_goods = action_goods.Count;
                int total_pages = (int)Math.Ceiling((decimal)total_goods / (decimal)quantity);

                if (page_num == 0) page_num = 1;
                if (page_num > total_pages) page_num = total_pages;

                int skip = (page_num - 1) * quantity;
                
                action_goods = action_goods.Skip(skip).Take(quantity).ToList();
                
                foreach (var article in action_goods)
                {
                    articles.Add(ArticleToDtoLight(article, lang));
                }

                var result = new
                {
                    data = new
                    {
                        articles,
                        total_goods,
                        total_pages,
                        displayed_page = page_num,
                        breadcrumbs = new List<dynamic> { new { id = 0, name = lang == "UK" ? "Гарячі новинки" : "Гарячие новинки" } }
                    }
                };

                return new JsonResult(result);
            }
            else
            {
                return BadRequest("Check  parameters!");
            }
            
        }


        /// <summary>
        /// Gets most favorable articles from favorites list
        /// </summary>
        /// <param name="goods_on_page">goods count to display</param>
        /// <param name="page">page to display starts from 1</param>
        /// <param name="lang">language</param>
        /// <returns>List of most favorable articles</returns>
        /// <response code="400">If the request value is bad</response>
        [HttpGet]
        [Route("most-favorites")]
        [ResponseCache(VaryByQueryKeys = new[] { "lang", "goods_on_page", "page" }, VaryByHeader = "User-Agent", Duration = 60)]
        public async Task<IActionResult> MostFavorable([FromQuery]  string lang, int goods_on_page=10, int page=1)
        {
            lang = lang.NormalizeLang();

            var articles = new List<dynamic>();

            var rand = new Random();

            var action_goods = await _context.Articles.
                Include(a => a.Title).ThenInclude(t => t.Translations).
                Include(a => a.Category).
                OrderByDescending(a => a.rztk_art_id).Skip(rand.Next(1000)).Take(100).ToListAsync();


            int total_goods = action_goods.Count;
            int total_pages = (int)Math.Ceiling((decimal)total_goods / (decimal)goods_on_page);

            if (page > total_pages) page = total_pages;

            int skip = (page - 1) * goods_on_page;
            
            action_goods = action_goods.Skip(skip).Take(goods_on_page).ToList();
            
            foreach (var article in action_goods)
            {
                articles.Add(ArticleToDtoLight(article, lang));
            }

            var result = new
            {
                data = new
                {
                    articles,
                    total_goods,
                    total_pages,
                    displayed_page = page,
                    breadcrumbs = new List<dynamic> { new {id=0, name = lang == "UK"? "Найчастіше додають в лист бажань": "Чаще всего добавляют в список желаний" } }
                }
            };

            return new JsonResult(result);
        }


        /// <summary>
        /// Gets most awaitable articles from wait lists
        /// </summary>
        /// <param name="goods_on_page">goods count to display</param>
        /// <param name="page">page to display starts from 1</param>
        /// <param name="lang">language</param>
        /// <returns>List of most awaitable articles </returns>
        /// <response code="400">If the request value is bad</response>
        [HttpGet]
        [Route("most-awaitable")]
        [ResponseCache(VaryByQueryKeys = new[] { "lang", "goods_on_page", "page" }, VaryByHeader = "User-Agent", Duration = 60)]
        public async Task<IActionResult> MostAwaitable([FromQuery] string lang, int goods_on_page=10, int page=1)
        {
            lang = lang.NormalizeLang();

            var articles = new List<dynamic>();

            var rand = new Random();

            var action_goods = await _context.Articles.
                Include(a => a.Title).ThenInclude(t => t.Translations).
                Include(a => a.Category).
                OrderByDescending(a => a.DocketId).Skip(rand.Next(1000)).Take(100).ToListAsync();
            
            int total_goods = action_goods.Count;
            int total_pages = (int)Math.Ceiling((decimal)total_goods / (decimal)goods_on_page);
            
            if (page > total_pages) page = total_pages;

            int skip = (page - 1) * goods_on_page;

            action_goods = action_goods.Skip(skip).Take(goods_on_page).ToList();

            foreach (var article in action_goods)
            {
                articles.Add(ArticleToDtoLight(article, lang));
            }

            var result = new
            {
                data = new
                {
                    articles,
                    total_goods,
                    total_pages,
                    displayed_page = page,
                    breadcrumbs = new List<dynamic> { new { id = 0, name = lang == "UK" ? "Найбільш очікувані" : "Наиболее ожидаемые" } }
                }
            };

            return new JsonResult(result);
        }

        /// <summary>
        /// Get articles by list of id (for example last of seen)
        /// </summary>
        /// <param name="articles_Ids">List of id</param>
        /// <param name="lang">language</param>
        /// <returns>Returns articles list</returns>
        /// <response code="400">If the request value is bad or list is empty</response>
        [HttpPost]
        [Route("from-list")]
        public async Task<IActionResult> GetByIdList([FromBody] List<int> articles_Ids, [FromQuery] string lang)
        {
            lang = lang.NormalizeLang();

            var articles = new List<dynamic>();

            //var articles_Ids = new List<int>();

            if (articles_Ids != null && articles_Ids.Count > 0)
            {

                var action_goods = await _context.Articles.
                    Include(a => a.Breadcrumbs).ThenInclude(b => b.Title).ThenInclude(t => t.Translations).
                    Include(a => a.Title).ThenInclude(t => t.Translations).
                    Include(a => a.Description).ThenInclude(t => t.Translations).
                    Include(a => a.Docket).ThenInclude(t => t.Translations).
                    Include(a => a.Brand).
                    Include(a => a.Seller).
                    Include(a => a.Warning).ThenInclude(w => w.Message).ThenInclude(m => m.Translations).
                    Include(a => a.Video).
                    Include(a => a.Actions).ThenInclude(a => a.Name).ThenInclude(n => n.Translations).
                    Where(a => articles_Ids.Contains(a.Id)).ToListAsync();



                foreach (var article in action_goods)
                {

                    articles.Add(ArticleToDto(article, lang));

                }

                var result = new
                {
                    data = new
                    {
                        articles

                    }
                };

                return new JsonResult(result);
            }
            else
            {
                return BadRequest("Check  parameters!");
            }


        }



        /// <summary>
        /// Get articles by list of id (for example last of seen)
        /// </summary>
        /// <param name="articles_Ids">List of id</param>
        /// <param name="lang">language</param>
        /// <returns>Returns articles list</returns>
        /// <response code="400">If the request value is bad or list is empty</response>
        [HttpPost]
        [Route("from-list-light")]
        public async Task<IActionResult> GetByIdListLight([FromBody] List<int> articles_Ids, [FromQuery] string lang)
        {
            lang = lang.NormalizeLang();

            var articles = new List<dynamic>();

            if (articles_Ids != null && articles_Ids.Count > 0)
            {

                var action_goods = await _context.Articles.
                    Include(a => a.Title).ThenInclude(t => t.Translations).
                    Include(a => a.Category).
                    Include(a => a.Actions).ThenInclude(a => a.Name).ThenInclude(n => n.Translations).
                    Where(a => articles_Ids.Contains(a.Id)).ToListAsync();
                
                foreach (var article in action_goods)
                {

                    articles.Add(ArticleToDtoLight(article, lang));

                }

                var result = new
                {
                    data = new
                    {
                        articles

                    }
                };

                return new JsonResult(result);
            }
            else
            {
                return BadRequest("Check  parameters!");
            }


        }

        /// <summary>
        /// Get minimal article data for cart list
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lang"></param>
        /// <returns>Article with name, preview and price</returns>
        [HttpGet]
        [Route("cart-item")]
        public async Task<IActionResult> GetArticleForCart([FromQuery] int id, string lang)
        {
            lang = lang.NormalizeLang();

            if (id == 0) return BadRequest(new Result
            {
                Status = "Error",
                Message = "Provide real id"
            });

            var article = await _context.Articles.Include(a => a.Title.Translations).
                Include(a => a.Images).ThenInclude(i => i.preview).
                AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);

            if (article == null) return NotFound("No such article!");

            var result = new
            {
                id = article.Id,
                name = article.Title.Content(lang),
                price = article.Price,
                small_img = Request.GetImageURL(BucketNames.preview.ToString(), article.Images.First().preview.url)
            };

            return new JsonResult(new { data = result });
        }

        private dynamic ArticleToDto(ArticleModel article, string lang)
        {
            dynamic dresponse = new ExpandoObject();
            IDictionary<string, object> response = (IDictionary<string, object>)dresponse;

            //create images
            // image = group of pictures
            // picture = one sample of concrete size
            var out_images = new List<dynamic>();

            var baseUrl = Request.Scheme + "://" + Request.Host + "/api/Goods/";

            var art_images = _context.Images.
                Include(i => i.original).
                 Include(i => i.base_action).
                  Include(i => i.preview).
                   Include(i => i.small).
                    Include(i => i.medium).
                     Include(i => i.large).
                      Include(i => i.big_tile).
                       Include(i => i.big).
                        Include(i => i.mobile_large).
                         Include(i => i.mobile_medium).
                            Where(i => i.ArticleModelId == article.Id).ToList();

            foreach (var image in art_images)
            {
                dynamic out_image_entity = new ExpandoObject();
                ((IDictionary<string, object>)out_image_entity).Add("id", image.Id);

                ((IDictionary<string, object>)out_image_entity).Add(nameof(image.original),
                 new
                 {
                     url = baseUrl + nameof(image.original) + "/" + image.original.url + ".jpg",
                     width = image.original.width,
                     height = image.original.height,
                 });

                ((IDictionary<string, object>)out_image_entity).Add(nameof(image.base_action),
                 new
                 {
                     url = baseUrl + nameof(image.base_action) + "/" + image.base_action.url + ".jpg",
                     width = image.base_action.width,
                     height = image.base_action.height,
                 });

                ((IDictionary<string, object>)out_image_entity).Add(nameof(image.preview),
                 new
                 {
                     url = baseUrl + nameof(image.preview) + "/" + image.preview.url + ".jpg",
                     width = image.preview.width,
                     height = image.preview.height,
                 });

                ((IDictionary<string, object>)out_image_entity).Add(nameof(image.small),
                 new
                 {
                     url = baseUrl + nameof(image.small) + "/" + image.small.url + ".jpg",
                     width = image.small.width,
                     height = image.small.height,
                 });

                ((IDictionary<string, object>)out_image_entity).Add(nameof(image.medium),
                 new
                 {
                     url = baseUrl + nameof(image.medium) + "/" + image.medium.url + ".jpg",
                     width = image.medium.width,
                     height = image.medium.height,
                 });

                ((IDictionary<string, object>)out_image_entity).Add(nameof(image.large),
                 new
                 {
                     url = baseUrl + nameof(image.large) + "/" + image.large.url + ".jpg",
                     width = image.large.width,
                     height = image.large.height,
                 });

                ((IDictionary<string, object>)out_image_entity).Add(nameof(image.big_tile),
                 new
                 {
                     url = baseUrl + nameof(image.big_tile) + "/" + image.big_tile.url + ".jpg",
                     width = image.big_tile.width,
                     height = image.big_tile.height,
                 });

                ((IDictionary<string, object>)out_image_entity).Add(nameof(image.big),
                 new
                 {
                     url = baseUrl + nameof(image.big) + "/" + image.big.url + ".jpg",
                     width = image.big.width,
                     height = image.big.height,
                 });

                ((IDictionary<string, object>)out_image_entity).Add(nameof(image.mobile_large),
                 new
                 {
                     url = baseUrl + nameof(image.mobile_large) + "/" + image.mobile_large.url + ".jpg",
                     width = image.mobile_large.width,
                     height = image.mobile_large.height,
                 });

                ((IDictionary<string, object>)out_image_entity).Add(nameof(image.mobile_medium),
                 new
                 {
                     url = baseUrl + nameof(image.mobile_medium) + "/" + image.mobile_medium.url + ".jpg",
                     width = image.mobile_medium.width,
                     height = image.mobile_medium.height,
                 });

                out_images.Add(out_image_entity);
            }

            //breadcrumbs collect
            var out_breadcrumbs = new List<dynamic>();

            out_breadcrumbs.Add(new
            {
                id = article.Id,
                name = article.Title.Content(lang),

            });

            int? curr_id = article.CategoryId;

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
                out_breadcrumbs.Add(newcat);

                curr_id = parent_cat.ParentCategoryId;

            } while (curr_id != null);


            var out_warnings = new List<dynamic>();

            foreach (var item in article.Warning)
            {
                var piece = new
                {
                    title = item.Message.Translations.First(t => t.LanguageId == lang).TranslationString ?? item.Message.OriginalText ?? "",
                };

                out_warnings.Add(piece);
            }

            //videos collect

            var out_videos = new List<dynamic>();

            foreach (var item in article.Video)
            {
                var piece = new
                {
                    name = item.Title,
                    id = item.Id,
                    type = item.Type,
                    ext_video_id = item.ExternalId,
                    title = item.Title,
                    url = item.URL,
                    preview_url = item.PreviewURL,
                    order = item.Order
                };

                out_videos.Add(piece);
            }



            //actions collect
            var actions = new List<dynamic>();
            if (article.Actions.Count > 0)
            {
                foreach (var action in article.Actions)
                {
                    var act = new
                    {
                        Name = action.Name.Content(lang),

                    };
                    actions.Add(act);
                }
            }

            var seller = new
            {
                seller_id = article.SellerId,
                seller_name = (article.Seller ?? new SellerModel()).Name,
                seller_rate = (article.Seller ?? new SellerModel()).Rate
            };


            response.Add("id", article.Id);
            response.Add("price", article.Price.ToString());
            response.Add("old_price", article.OldPrice.ToString());
            response.Add("title", article.Title.Content(lang));
            response.Add("description", article.Description.Content(lang));
            response.Add("status", article.Status ?? "");
            response.Add("sell_status", article.SellStatus ?? "");
            response.Add("seller", seller);
            response.Add("comments_amount", 0);
            response.Add("category_id", article.CategoryId);
            response.Add("top_category_id", article.TopCategoryId ?? 0);
            response.Add("docket", article.Docket.Content(lang));
            response.Add("brand_id", article.Brand.Id);
            response.Add("brand", article.Brand.Name ?? "");
            response.Add("brand_name", article.Brand.Description ?? "");
            response.Add("brand_logo", baseUrl + "logo/" + article.Brand.LogoURL + ".jpg");
            response.Add("breadcrumbs", out_breadcrumbs);
            response.Add("images", out_images);

            response.Add("warning", out_warnings);
            response.Add("videos", out_videos);
            response.Add("actions", actions);
            response.Add("tags", new object[] { });

            return response;
        }

        private dynamic ArticleToDtoLight(ArticleModel article, string lang)
        {
            dynamic dresponse = new ExpandoObject();
            IDictionary<string, object> response = (IDictionary<string, object>)dresponse;

            //create images
            // image = group of pictures
            // picture = one sample of concrete size
            var out_images = new List<dynamic>();

            var baseUrl = Request.Scheme + "://" + Request.Host + "/api/Goods/";

            var art_images = _context.Images.
                Include(i => i.preview).
                  Where(i => i.ArticleModelId == article.Id).ToList();

            var preview = art_images.First(); //todo find out how deal to no images added to article (add one no-picture at article creation?)

            var preview_img = new
            {
                url = baseUrl + nameof(preview.preview) + "/" + preview.preview.url + ".jpg",
                width = preview.preview.width,
                height = preview.preview.height,
            };
            
            //actions collect
            var actions = new List<dynamic>();
            if (article.Actions.Count > 0)
            {
                foreach (var action in article.Actions)
                {
                    var act = new
                    {
                        Name = action.Name.Content(lang),

                    };
                    actions.Add(act);
                }
            }
            
            response.Add("id", article.Id);
            response.Add("price", article.Price.ToString());
            response.Add("old_price", article.OldPrice.ToString());
            response.Add("title", article.Title.Content(lang));
            response.Add("sell_status", article.SellStatus ?? "");
            response.Add("category_id", article.CategoryId);
            response.Add("preview_img", preview_img);
            response.Add("actions", actions);
            response.Add("points", article.Points);

            return response;
        }
    }
}
