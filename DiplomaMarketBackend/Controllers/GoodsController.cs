using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;

namespace DiplomaMarketBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoodsController : Controller
    {
        ILogger<WorkController> _logger;
        ICloudStorageService _storageService;
        BaseContext _context;
        IFileService _fileService;

        public GoodsController(ILogger<WorkController> logger, ICloudStorageService cloudStorageService, BaseContext context, IFileService fileService)
        {
            _logger = logger;
            _storageService = cloudStorageService;
            _context = context;
            _fileService = fileService;
        }


        /// <summary>
        /// Get all articles ids from given category
        /// </summary>
        /// <param name="lang">language (e.g. uk\ru)</param>
        /// <param name="category_Id">id of category</param>
        /// <returns>json result with list of ids on key "ids"</returns>
        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> GetCategoryArticles([FromQuery] string lang, string category_Id)
        {
            int goods_limit = 60;

            if (int.TryParse(category_Id, out int id))
            {

                var category = await _context.Categories.Include(c => c.ChildCategories).ThenInclude(c => c.ChildCategories).FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    return NotFound("No such category");
                }

                var all_cat_list = category.ChildCategories.SelectMany(x => x.ChildCategories).Concat(category.ChildCategories.SelectMany(x => x.ChildCategories).SelectMany(x => x.ChildCategories)).ToList();
                all_cat_list.Add(category);

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

            return BadRequest("Check request values!");
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
        public async Task<IActionResult> GetArticleFull([FromQuery] string lang, string goodsId)
        {
            if (lang.ToUpper() != "UK" && lang.ToUpper() != "RU")
                lang = "UA";

            dynamic dresponse = new ExpandoObject();
            IDictionary<string, object> response = (IDictionary<string, object>)dresponse;

            if (int.TryParse(goodsId, out int art_id))
            {

                var article = await _context.Articles.
                    Include(a => a.Breadcrumbs).ThenInclude(b => b.Title).ThenInclude(t => t.Translations).
                    Include(a => a.Title).ThenInclude(t => t.Translations).
                    Include(a => a.Description).ThenInclude(t => t.Translations).
                    Include(a => a.Docket).ThenInclude(t => t.Translations).
                    Include(a => a.Brand).
                    Include(a => a.Warning).ThenInclude(w => w.Message).ThenInclude(m => m.Translations).
                    Include(a => a.Video).
                    //Include(a => a.Images).
                    FirstOrDefaultAsync(a => a.Id == art_id);

                if (article == null)
                {
                    return NotFound($"Article id={goodsId} not exist!");
                }

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
                                Where(i => i.ArticleModelId == art_id).ToList();

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

                foreach (var item in article.Breadcrumbs)
                {
                    var piece = new
                    {
                        id = item.Id,
                        title = item.Title.Content(lang),
                        href = "todo"
                    };

                    out_breadcrumbs.Add(piece);
                }

                //warnings collect

                var out_warnings = new List<dynamic>();

                foreach (var item in article.Warning)
                {
                    var piece = new
                    {
                        title = item.Message.Translations.First(t => t.LanguageId == lang.ToUpper()).TranslationString ?? item.Message.OriginalText ?? "",
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


                response.Add("id", article.Id);
                response.Add("price", article.Price.ToString());
                response.Add("old_price", article.OldPrice.ToString());
                response.Add("title", article.Title.Content(lang));
                response.Add("description", article.Description.Content(lang));
                response.Add("status", article.Status ?? "");
                response.Add("sell_status", article.SellStatus ?? "");
                response.Add("comments_amount", 0);
                response.Add("category_id", article.CategoryId);
                response.Add("docket", article.Docket.Content(lang));

                response.Add("brand", article.Brand.Name ?? "");
                response.Add("brand_name", article.Brand.Description ?? "");
                response.Add("brand_logo", baseUrl + "logo/" + article.Brand.LogoURL + ".jpg");
                response.Add("breadcrumbs", out_breadcrumbs);
                response.Add("images", out_images);

                response.Add("warning", out_warnings);
                response.Add("videos", out_videos);

                response.Add("tags", new object[] { });


            }
            else
            {
                return BadRequest($"Request value id={goodsId} must be a number!");
            }

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
        public async Task<IActionResult> GetArticleCharacteristics([FromQuery] string lang, string goodsId)
        {
            if (lang.ToUpper() != "UK" && lang.ToUpper() != "RU")
                lang = "UA";


            var characteristic_group_list = new List<dynamic>();


            if (int.TryParse(goodsId, out int art_id))
            {
                var values = await _context.Values.
                Include(v => v.Title).ThenInclude(t => t.Translations).
                Include(v => v.CharacteristicType).ThenInclude(t => t.Group).ThenInclude(g => g.groupTitle).ThenInclude(t => t.Translations).
                Include(v => v.CharacteristicType).ThenInclude(v => v.Name).ThenInclude(v => v.Translations).
                Include(v => v.CharacteristicType).ThenInclude(v => v.Title).ThenInclude(v => v.Translations).
                Where(c => c.articleId == art_id).
                ToListAsync();

                var groups = values.GroupBy(v => v.CharacteristicType).GroupBy(v => v.Key.Group).ToList();


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
                            var list_values = new List<dynamic>();

                            foreach (var val in charakterystyc_type.Key.Values)
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

                            foreach (var val in charakterystyc_type.Key.Values)
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
        /// Gets file by url from Mongo
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="id"></param>
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
    }
}
