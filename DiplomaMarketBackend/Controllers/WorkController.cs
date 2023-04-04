using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Models;
using DiplomaMarketBackend.Parser;
using DiplomaMarketBackend.Parser.Categories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace DiplomaMarketBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkController : ControllerBase
    {
        ILogger<WorkController> _logger;
        ICloudStorageService _storageService;
        BaseContext _context;
        IFileService _fileService;

        public WorkController(ILogger<WorkController> logger, ICloudStorageService cloudStorageService, BaseContext context, IFileService fileService)
        {
            _logger = logger;
            _storageService = cloudStorageService;
            _context = context;
            _fileService = fileService;
        }

        [HttpPost]
        [Route("testSave")]
        public async Task<IActionResult> test()
        {

            var charakts = _context.ArticleCharacteristics.Include(c=>c.Group).ToList();


            foreach(var har in charakts)
            {
                if(har.Group != null)
                har.GroupId = har.Group.Id;
            }



            return Ok();

            _fileService.DeleteFile("fs", "6428277f976576a2e32d3bee");

            var art = new ArticleModel();

            var transl = new Translation() { LanguageId = "UK", TranslationString = "SomeTranslation" };

            _context.translations.Add(transl);

            var namecontent = new TextContent { OriginalLanguageId = "UK", OriginalText = "Some text" };
            namecontent.Translations.Add(transl);

            art.Title = namecontent;
            var desc = new TextContent()
            {
                OriginalLanguageId = "UK",
                OriginalText = "Any"
            };
            _context.textContents.Add(desc);

            art.Description = desc;

            var dock = new TextContent()
            {
                OriginalLanguageId = "UK",
                OriginalText = "Any"
            };

            _context.textContents.Add(dock);
            art.Docket = dock;

            var brand = new BrandModel()
            {

                Name = "Any",
                Description = "Any",
                LogoURL = "324535747"
            };

            _context.Brands.Add(brand);

            art.Brand = brand;

            _context.Articles.Add(art);
            _context.SaveChanges();




            return Ok();

        }

        [HttpPost]
        public async Task<IActionResult> upload(IFormFile file)
        {



            var url = _storageService.UploadFileAsync(file, file.FileName).Result.ToString();


            return Ok($"Received file {file.FileName} with size in bytes {file.Length} and url is {url}");
        }

        [HttpPost]
        [Route("createCategory")]
        public async Task<IActionResult> CreateCategory([FromBody] dynamic json, [FromQuery] string lang)
        {

            dynamic jdata = JsonConvert.DeserializeObject<dynamic>(json.ToString());

            var data = jdata["data"];
            Language defLanguage = _context.Languages.First(l => l.Id == lang.ToUpper());

            //_context.Categories.RemoveRange(_context.Categories.ToList());
            //_context.SaveChanges();

            foreach (var key in data)
            {
                var value = key.Value;
                Parser.Categories.Category category = JsonConvert.DeserializeObject<Parser.Categories.Category>(value.ToString());

                var top = GetCategory(category, defLanguage, null);




                if (category.children != null)
                {
                    if (category.children.one != null)
                    {
                        foreach (var cat_2L in category.children.one)
                        {

                            var middle = GetCategory(cat_2L, defLanguage, top);


                            if (cat_2L.children != null)
                            {
                                foreach (var cat_3L in cat_2L.children)
                                {
                                    var end = GetCategory(cat_3L, defLanguage, middle);



                                }
                            }

                        }
                    }

                    if (category.children.two != null)
                    {
                        foreach (var cat_2L in category.children.two)
                        {

                            var middle = GetCategory(cat_2L, defLanguage, top);


                            if (cat_2L.children != null)
                            {
                                foreach (var cat_3L in cat_2L.children)
                                {
                                    var end = GetCategory(cat_3L, defLanguage, middle);



                                }
                            }

                        }
                    }

                    if (category.children.three != null)
                    {
                        foreach (var cat_2L in category.children.three)
                        {

                            var middle = GetCategory(cat_2L, defLanguage, top);


                            if (cat_2L.children != null)
                            {
                                foreach (var cat_3L in cat_2L.children)
                                {
                                    var end = GetCategory(cat_3L, defLanguage, middle);



                                }
                            }

                        }
                    }
                }
            }

            return Ok($"Received json");
        }

        private CategoryModel GetCategory(ICategory foreignCategory, Language defLanguage, CategoryModel parent)
        {


            var new_cat = _context.Categories.Include(c => c.Name).FirstOrDefault(c => c.rztk_cat_id == foreignCategory.category_id);

            if (new_cat == null)
            {
                var translation = new Translation()
                {
                    TranslationString = foreignCategory.title,
                    Language = defLanguage,
                };

                _context.translations.Add(translation);


                TextContent new_text = new TextContent()
                {
                    OriginalLanguage = defLanguage,
                    OriginalText = foreignCategory.title,
                };

                new_text.Translations.Add(translation);

                _context.textContents.Add(new_text);



                new_cat = new CategoryModel()
                {
                    Name = new_text,
                    rztk_cat_id = foreignCategory.category_id,
                    ParentCategory = parent
                };

                _logger.Log(LogLevel.Information, "Category added > " + new_cat.Name);
                _context.Categories.Add(new_cat);
                _context.SaveChanges();

            }
            else
            {
                if (new_cat != null)
                {
                    var translation = new Translation()
                    {
                        TranslationString = foreignCategory.title,
                        Language = defLanguage,
                    };

                    _context.translations.Add(translation);


                    if (new_cat.Name != null)
                        new_cat.Name.Translations.Add(translation);
                    _context.SaveChanges();
                }

            }

            return new_cat;

        }

        [HttpPost]
        [Route("parseArticles")]
        public async Task<IActionResult> ParseArticles([FromHeader] string password)
        {
            if (password != "pass123") return Ok("pass not pass");

            var last_category = _context.Articles.FirstOrDefault(a => a.CategoryId == _context.Articles.Max(a => a.CategoryId)).CategoryId;

            var categoryes = _context.Categories.Where(c => c.Id > last_category).ToList();

            var languages = new string[] { "UK", "RU" };

            string responseBody = "";

            foreach (var category in categoryes)
            {
                string cat_point = "https://xl-catalog-api.rozetka.com.ua/v4/goods/get?front-type=xl&country=UA&lang=ru&category_id=" + category.rztk_cat_id.ToString();

                try
                {
                    var client = new HttpClient();
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, cat_point);

                    var response = await client.SendAsync(request);
                    responseBody = await response.Content.ReadAsStringAsync();

                    _logger.LogWarning(cat_point + ": " + responseBody);

                    var myCategoryClass = JsonConvert.DeserializeObject<DiplomaMarketBackend.Parser.Category.Root>(responseBody);

                    if (myCategoryClass == null) continue;

                    var ids = myCategoryClass.data.ids.Take(20);

                    foreach (var id in ids)
                    {
                        ArticleModel? new_article = null;
                        string aresponseBody = "";
                        string char_responseBody = "";

                        try
                        {

                            foreach (var lang in languages)
                            {

                                //get article
                                string endpoint = "https://rozetka.com.ua/api/product-api/v4/goods/get-main?front-type=xl&country=UA&lang=" + lang.ToLower() + "&goodsId=" + id;
                                var art_client = new HttpClient();
                                HttpRequestMessage artrequest = new HttpRequestMessage(HttpMethod.Get, endpoint);

                                var aresponse = await client.SendAsync(artrequest);
                                aresponseBody = await aresponse.Content.ReadAsStringAsync();

                                var article = JsonConvert.DeserializeObject<DiplomaMarketBackend.Parser.Article.Root>(aresponseBody);

                                //get characteristic
                                string char_endpoint = "https://rozetka.com.ua/api/product-api/v4/goods/get-characteristic?front-type=xl&country=UA&lang=" + lang.ToLower() + "&goodsId=" + 245161909;
                                var char_client = new HttpClient();
                                HttpRequestMessage char_request = new HttpRequestMessage(HttpMethod.Get, char_endpoint);

                                var char_response = await client.SendAsync(char_request);
                                char_responseBody = await char_response.Content.ReadAsStringAsync();

                                var char_article = JsonConvert.DeserializeObject<DiplomaMarketBackend.Parser.Characteristics.Root>(char_responseBody);



                                if (article == null || char_article == null) continue;

                                new_article = _context.Articles.FirstOrDefault(a => a.rztk_art_id == id);

                                if (new_article == null)
                                {
                                    new_article = new ArticleModel();

                                    new_article.rztk_art_id = id;
                                    new_article.Category = category;
                                    new_article.Title = TextContentHelper.CreateTextContent(_context, article.data.title, lang);
                                    new_article.Price = decimal.Parse(article.data.price);
                                    new_article.OldPrice = decimal.Parse(article.data.old_price);
                                    new_article.Description = TextContentHelper.CreateTextContent(_context, article.data.description.text, lang);
                                    new_article.Docket = TextContentHelper.CreateTextContent(_context, article.data.docket, lang);
                                    new_article.Updated = DateTime.Now;


                                    BrandModel? brand = _context.Brands.FirstOrDefault(b => b.rztk_brand_id == article.data.brand_id);

                                    if (brand == null && article.data.brand_id != null)
                                    {

                                        brand = new BrandModel()
                                        {

                                            Name = article.data.brand,
                                            Description = article.data.brand_name,

                                            rztk_brand_id = article.data.brand_id,
                                        };

                                        if (article.data.brand_logo != null)
                                        {
                                            brand.LogoURL = _fileService.SaveFileFromUrl("logo", article.data.brand_logo).Result;
                                        }

                                        _context.Brands.Add(brand);

                                    }

                                    new_article.Brand = brand;

                                    new_article.Status = "active";
                                    new_article.SellStatus = "available";

                                    //Warnings add
                                    if (article.data.warning != null)
                                    {
                                        new_article.Warning = new List<WarningModel>();

                                        foreach (var warning in article.data.warning)
                                        {
                                            var warn = new WarningModel()
                                            {
                                                Message = TextContentHelper.CreateTextContent(_context, warning, lang)
                                            };
                                            _context.Warnings.Add(warn);
                                            new_article.Warning.Add(warn);
                                        }


                                    }

                                    //Videos add
                                    if (article.data.videos != null && article.data.videos.Count != 0)
                                    {
                                        foreach (var videos in article.data.videos)
                                        {
                                            if (videos.type == "rozetka") continue;

                                            var new_video = new VideoModel()
                                            {
                                                Title = videos.title,
                                                URL = videos.url,
                                                Type = videos.type,
                                                PreviewURL = videos.preview_url,
                                                ExternalId = videos.ext_video_id
                                            };

                                            _context.Videos.Add(new_video);

                                            new_article.Video.Add(new_video);
                                        }
                                    }

                                    //Images add
                                    if (article.data.images != null && article.data.images.Count != 0)
                                    {
                                        foreach (var image in article.data.images)
                                        {
                                            var new_image = new ImageModel();

                                            PictureModel original = new PictureModel()
                                            {
                                                width = image.original.width,
                                                height = image.original.height,
                                                url = _fileService.SaveFileFromUrl("original", image.original.url).Result
                                            };

                                            PictureModel base_action = new PictureModel()
                                            {
                                                width = image.base_action.width,
                                                height = image.base_action.height,
                                                url = _fileService.SaveFileFromUrl("base_action", image.base_action.url).Result
                                            };

                                            PictureModel preview = new PictureModel()
                                            {
                                                width = image.preview.width,
                                                height = image.preview.height,
                                                url = _fileService.SaveFileFromUrl("preview", image.preview.url).Result
                                            };

                                            PictureModel small = new PictureModel()
                                            {
                                                width = image.small.width,
                                                height = image.small.height,
                                                url = _fileService.SaveFileFromUrl("bigTile", image.small.url).Result
                                            };

                                            PictureModel medium = new PictureModel()
                                            {
                                                width = image.medium.width,
                                                height = image.medium.height,
                                                url = _fileService.SaveFileFromUrl("medium", image.medium.url).Result
                                            };

                                            PictureModel large = new PictureModel()
                                            {
                                                width = image.large.width,
                                                height = image.large.height,
                                                url = _fileService.SaveFileFromUrl("large", image.large.url).Result
                                            };

                                            PictureModel big_tile = new PictureModel()
                                            {
                                                width = image.big_tile.width,
                                                height = image.big_tile.height,
                                                url = _fileService.SaveFileFromUrl("big_tile", image.big_tile.url).Result
                                            };

                                            PictureModel big = new PictureModel()
                                            {
                                                width = image.big.width,
                                                height = image.big.height,
                                                url = _fileService.SaveFileFromUrl("big", image.big.url).Result
                                            };

                                            PictureModel mobile_medium = new PictureModel()
                                            {
                                                width = image.mobile_medium.width,
                                                height = image.mobile_medium.height,
                                                url = _fileService.SaveFileFromUrl("mobile_medium", image.mobile_medium.url).Result
                                            };

                                            PictureModel mobile_large = new PictureModel()
                                            {
                                                width = image.mobile_large.width,
                                                height = image.mobile_large.height,
                                                url = _fileService.SaveFileFromUrl("mobile_large", image.mobile_large.url).Result
                                            };


                                            _context.Pictures.Add(original);
                                            _context.Pictures.Add(base_action);
                                            _context.Pictures.Add(preview);
                                            _context.Pictures.Add(small);
                                            _context.Pictures.Add(medium);
                                            _context.Pictures.Add(large);
                                            _context.Pictures.Add(big_tile);
                                            _context.Pictures.Add(big);
                                            _context.Pictures.Add(mobile_medium);
                                            _context.Pictures.Add(mobile_large);

                                            new_image.original = original;
                                            new_image.base_action = base_action;
                                            new_image.preview = preview;
                                            new_image.small = small;
                                            new_image.medium = medium;
                                            new_image.large = large;
                                            new_image.big_tile = big_tile;
                                            new_image.big = big;
                                            new_image.mobile_medium = mobile_medium;
                                            new_image.mobile_large = mobile_large;

                                            _context.Images.Add(new_image);

                                            new_article.Images.Add(new_image);

                                        }

                                    }

                                    //breadcrumbs
                                    if (article.data.breadcrumbs != null && article.data.breadcrumbs.Count > 0)
                                    {
                                        foreach (var breadcrumb in article.data.breadcrumbs)
                                        {
                                            var exst_bread = _context.Breadcrumbs.FirstOrDefault(b => b.roz_bread_id == breadcrumb.id);

                                            if (exst_bread == null)
                                            {

                                                exst_bread = new BreadcrumbsModel()
                                                {

                                                    Title = TextContentHelper.CreateTextContent(_context, breadcrumb.title, lang),
                                                    roz_bread_id = breadcrumb.id,
                                                };

                                                _context.Breadcrumbs.Add(exst_bread);
                                            }

                                            exst_bread.roz_bread_id = breadcrumb.id;
                                            new_article.Breadcrumbs.Add(exst_bread);
                                        }
                                    }

                                    //Parse  characteristics
                                    foreach (var char_group in char_article.data)
                                    {
                                        if (char_group.group_id != 0)
                                        {
                                            var exst_gpr = _context.CharacteristicGroups.FirstOrDefault(c => c.rztk_grp_id == char_group.group_id);

                                            if (exst_gpr == null)
                                            {
                                                exst_gpr = new CharacteristicGroupModel()
                                                {
                                                    groupTitle = TextContentHelper.CreateTextContent(_context, char_group.groupTitle, lang),
                                                    group_order = char_group.group_order,
                                                    rztk_grp_id = char_group.group_id,
                                                };

                                                _context.CharacteristicGroups.Add(exst_gpr);

                                                foreach (var harakt in char_group.options)
                                                {
                                                    var new_charackt = new ArticleCharacteristic()
                                                    {
                                                        Title = TextContentHelper.CreateTextContent(_context, harakt.title, lang),
                                                        Name = TextContentHelper.CreateTextContent(_context, harakt.name, lang),
                                                        Status = harakt.status,
                                                        CategoryId = category.Id,
                                                        Order = harakt.order,
                                                        Group = exst_gpr,
                                                        roz_har_id = harakt.id


                                                    };

                                                    if (Enum.TryParse(typeof(CharacteristicType), harakt.type, out object? type))
                                                    {
                                                        new_charackt.Type = (CharacteristicType)type;
                                                    }



                                                    foreach (var val in harakt.values)
                                                    {
                                                        var new_value = new CharacteristicValueModel()
                                                        {
                                                            Title = TextContentHelper.CreateTextContent(_context, val.title, lang)
                                                        };

                                                        _context.Values.Add(new_value);
                                                        new_charackt.Values.Add(new_value);
                                                    }

                                                    _context.ArticleCharacteristics.Add(new_charackt);

                                                    new_article.ArticleCharacteristics.Add(new_charackt);

                                                }

                                            }
                                        }
                                        else
                                        {

                                            foreach (var harakt in char_group.options)
                                            {
                                                var new_charackt = new ArticleCharacteristic()
                                                {
                                                    Title = TextContentHelper.CreateTextContent(_context, harakt.title, lang),
                                                    Name = TextContentHelper.CreateTextContent(_context, harakt.name, lang),
                                                    Status = harakt.status,
                                                    CategoryId = category.Id,
                                                    Order = harakt.order,
                                                    roz_har_id = harakt.id
                                                };

                                                if (Enum.TryParse(typeof(CharacteristicType), harakt.type, out object? type))
                                                {
                                                    new_charackt.Type = (CharacteristicType)type;
                                                }

                                                foreach (var val in harakt.values)
                                                {
                                                    var new_value = new CharacteristicValueModel()
                                                    {
                                                        Title = TextContentHelper.CreateTextContent(_context, val.title, lang)
                                                    };

                                                    _context.Values.Add(new_value);
                                                    new_charackt.Values.Add(new_value);
                                                }

                                                _context.ArticleCharacteristics.Add(new_charackt);

                                                new_article.ArticleCharacteristics.Add(new_charackt);

                                            }
                                        }
                                    }

                                    _context.Articles.Add(new_article);
                                    _logger.LogInformation("#" + new_article.Id + " added ");
                                }
                                else if (new_article != null && lang != "UK") //if article exist
                                {

                                    TextContentHelper.UpdateTextContent(_context, article.data.title, new_article.TitleId, lang);
                                    TextContentHelper.UpdateTextContent(_context, article.data.description.text, new_article.DescriptionId, lang);
                                    TextContentHelper.UpdateTextContent(_context, article.data.docket, new_article.DocketId, lang);
                                    new_article.Updated = DateTime.Now;


                                    //Warnings update
                                    if (article.data.warning != null)
                                    {

                                        if (new_article.Warning != null && new_article.Warning.Count == article.data.warning.Count)
                                        {
                                            for (int i = 0; i < new_article.Warning.Count; i++)
                                            {
                                                TextContentHelper.UpdateTextContent(_context, article.data.warning[i], new_article.Warning[i].Message.Id, lang);
                                            }

                                        }

                                    }


                                    //breadcrumbs update
                                    if (article.data.breadcrumbs != null && article.data.breadcrumbs.Count > 0)
                                    {
                                        foreach (var breadcrumb in article.data.breadcrumbs)
                                        {
                                            var exst_bread = _context.Breadcrumbs.FirstOrDefault(b => b.roz_bread_id == breadcrumb.id);

                                            TextContentHelper.UpdateTextContent(_context, breadcrumb.title, exst_bread?.TitleId, lang);

                                        }
                                    }

                                    //Update  characteristics
                                    foreach (var char_group in char_article.data)
                                    {
                                        if (char_group.group_id != 0)
                                        {
                                            var exst_gpr = _context.CharacteristicGroups.FirstOrDefault(c => c.rztk_grp_id == char_group.group_id);

                                            if (exst_gpr != null)
                                            {
                                                TextContentHelper.UpdateTextContent(_context, char_group.groupTitle, exst_gpr.groupTitle?.Id, lang);


                                                foreach (var harakt in char_group.options)
                                                {

                                                    var exst_charakt = _context.ArticleCharacteristics.FirstOrDefault(h => h.roz_har_id == harakt.id);

                                                    if (exst_charakt != null)
                                                    {
                                                        TextContentHelper.UpdateTextContent(_context, harakt.title, exst_charakt?.TitleId, lang);
                                                    }

                                                }

                                            }
                                        }
                                        else
                                        {
                                            int i = 0;
                                            var lone_characts = new_article.ArticleCharacteristics.Where(c => c.GroupId == 0).ToList();

                                            if (lone_characts.Count == char_group.options.Count)

                                                foreach (var harakt in char_group.options)
                                                {
                                                    TextContentHelper.UpdateTextContent(_context, harakt.title, lone_characts[i].TitleId, lang);
                                                    TextContentHelper.UpdateTextContent(_context, harakt.name, lone_characts[i].TitleId, lang);

                                                    int j = 0;
                                                    foreach (var val in harakt.values)
                                                    {
                                                        TextContentHelper.UpdateTextContent(_context, val.title, lone_characts[i].Values[j].TitleId, lang);
                                                        j++;
                                                    }

                                                    i++;
                                                }
                                        }
                                    }

                                    _logger.LogInformation("#" + new_article.Id + " updated ");

                                }

                                _context.SaveChanges();
                            }




                        }
                        catch (Exception e)
                        {
                            if (new_article != null && new_article.Images.Count > 0)
                            {

                                //remove any pictires
                                foreach (var image in new_article.Images)
                                {
                                    if (image.original != null)
                                        _fileService.DeleteFile("original", image.original.url ?? "");

                                    if (image.base_action != null)
                                        _fileService.DeleteFile("base_action", image.base_action.url ?? "");

                                    if (image.preview != null)
                                        _fileService.DeleteFile("preview", image.preview.url ?? "");

                                    if (image.small != null)
                                        _fileService.DeleteFile("small", image.small.url ?? "");

                                    if (image.medium != null)
                                        _fileService.DeleteFile("medium", image.medium.url ?? "");

                                    if (image.large != null)
                                        _fileService.DeleteFile("large", image.large.url ?? "");

                                    if (image.big_tile != null)
                                        _fileService.DeleteFile("big_tile", image.big_tile.url ?? "");

                                    if (image.big != null)
                                        _fileService.DeleteFile("big", image.big.url ?? "");

                                    if (image.mobile_medium != null)
                                        _fileService.DeleteFile("mobile_medium", image.mobile_medium.url ?? "");

                                    if (image.mobile_large != null)
                                        _fileService.DeleteFile("mobile_large", image.mobile_large.url ?? "");

                                }
                            }

                            _logger.LogWarning(e.Message + "\n\nArticle : " + aresponseBody + "\n\n Characteristics : " + char_responseBody + "\n\n Category articles : " + responseBody);

                            throw;

                        }

                    }

                }
                catch (JsonSerializationException e)//if category empty or else
                {
                    continue;
                }
                catch(Exception e)
                {
                    _logger.LogWarning(e.Message + "\n\nResponse : " + responseBody);
                   throw;
                }
            }


            return Ok($"Articles parsed");
        }

        [HttpPost]
        [Route("getCategories")]

        public async Task<IActionResult> GetCategories([FromQuery] string lang)
        {
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
                        }

                        output_mchild.Children.Add(output_lchild);
                    }

                    output_cat.Children.Add(output_mchild);
                }

                result.Add(output_cat);
            }


            return new JsonResult(result);
        }


        [HttpPost]
        [Route("saveFile")]
        public async Task<IActionResult> SaveFile([FromHeader] string url)
        {


            var id = _fileService.SaveFileFromUrl("fs", $"https://content.rozetka.com.ua/goods/images/preview/251021268.jpg");

            return Ok($"File saved" + id.ToString());
        }

        [HttpGet]
        [Route("big/{id}")]
        public async Task<IActionResult> bigImage(string id)
        {

            string MimeType = "image/jpg";
            var img_id = id.Split('.')[0];
            //var client = new MongoClient("mongodb://root:example@localhost:27017");
            //var db = client.GetDatabase("test");
            //IGridFSBucket gridFS = new GridFSBucket(db);

            //await gridFS.DownloadToStreamAsync(new ObjectId(id), context.Response.Body);
            //var bytes = await gridFS.DownloadAsBytesAsync(new ObjectId(img_id));
            var bytes = _fileService.GetFile(img_id, "fs").Result;
            return File(bytes, MimeType);
        }

    }
}
