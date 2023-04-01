using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Models;
using DiplomaMarketBackend.Parser;
using DiplomaMarketBackend.Parser.Article;
using DiplomaMarketBackend.Parser.Categories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Newtonsoft.Json;
using System.Linq;
using System.Net;

namespace DiplomaMarketBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        ILogger<FilesController> _logger;
        ICloudStorageService _storageService;
        BaseContext _context;
        IFileService _fileService;

        public FilesController(ILogger<FilesController> logger, ICloudStorageService cloudStorageService, BaseContext context, IFileService fileService)
        {
            _logger = logger;
            _storageService = cloudStorageService;
            _context = context;
            _fileService = fileService; 
        }

        [HttpPost]
        public async Task<IActionResult> upload(IFormFile file)
        {


            var url = _storageService.UploadFileAsync(file, file.FileName).Result.ToString();


            return Ok($"Received file {file.FileName} with size in bytes {file.Length} and url is {url}");
        }

        [HttpPost]
        [Route("createCategory")]
        public async Task<IActionResult> CreateCategory([FromBody] dynamic json)
        {

            dynamic jdata = await JsonConvert.DeserializeObject<dynamic>(json.ToString());

            var data = jdata["data"];
            Language defLanguage = _context.Languages.First(l => l.Id == "RU");

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
                _context.SaveChanges();

                TextContent new_text = new TextContent()
                {
                    OriginalLanguage = defLanguage,
                    OriginalText = foreignCategory.title,
                };

                new_text.Translations.Add(translation);

                _context.textContents.Add(new_text);
                _context.SaveChanges();


                new_cat = new CategoryModel()
                {
                    NameId = new_text.Id,
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
                    _context.SaveChanges();

                    if (new_cat.Name != null)
                        new_cat.Name.Translations.Add(translation);
                    _context.SaveChanges();
                }

            }

            return new_cat;

        }

        [HttpPost]
        [Route("parseArticles")]
        public async Task<IActionResult> ParseArticles()
        {
            var categoryes = _context.Categories.ToList();

            var languages = new string[]{ "UK", "RU" };

            foreach(var category in categoryes)
            {
                string cat_point = "https://xl-catalog-api.rozetka.com.ua/v4/goods/get?front-type=xl&country=UA&lang=ru&category_id=" + category.rztk_cat_id.ToString();


                var client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, cat_point);

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                var myCategoryClass = JsonConvert.DeserializeObject<DiplomaMarketBackend.Parser.Category.Root>(responseBody);

                if (myCategoryClass == null) continue;



                foreach(var id in myCategoryClass.data.ids)
                {
                    foreach(var lang in languages)
                    {
                        string endpoint = "https://rozetka.com.ua/api/product-api/v4/goods/get-main?front-type=xl&country=UA&lang="+lang+"&goodsId=" + id;
                        var art_client = new HttpClient();
                        HttpRequestMessage artrequest = new HttpRequestMessage(HttpMethod.Get, endpoint);

                        var aresponse = await client.SendAsync(artrequest);
                        var aresponseBody = await aresponse.Content.ReadAsStringAsync();

                        var article = JsonConvert.DeserializeObject<DiplomaMarketBackend.Parser.Article.Root>(aresponseBody);

                        if (article == null) continue;

                        var new_article = _context.Articles.FirstOrDefault(a => a.rztk_art_id == id);

                        if (new_article == null)
                        {
                            new_article = new ArticleModel();

                            new_article.rztk_art_id = id;
                            new_article.Category = category;
                            new_article.Title = TextContentHelper.CreateTextContent(_context,article.data.title,lang);
                            new_article.Price = decimal.Parse(article.data.price);
                            new_article.OldPrice = decimal.Parse(article.data.old_price);
                            new_article.Description = TextContentHelper.CreateTextContent(_context,article.data.description.text,lang);
                            new_article.Docket = TextContentHelper.CreateTextContent(_context, article.data.docket, lang);
                            new_article.Updated = DateTime.Now;


                            BrandModel? brand = _context.Brands.FirstOrDefault(b=>b.rztk_brand_id == article.data.brand_id);

                            if(brand == null)
                            {
                                brand = new BrandModel() {

                                    Name = article.data.brand,
                                    Description = article.data.brand_name,
                                    LogoURL = _fileService.SaveFileFromUrl("logo", article.data.brand_logo).Result,
                                    rztk_brand_id = article.data.brand_id,
                                };

                                _context.Brands.Add(brand); 
                                _context.SaveChanges();

                            }

                            new_article.Brand = brand;

                            new_article.Status = "active";
                            new_article.SellStatus = "available";

                            if(article.data.warning != null)
                            {
                                new_article.Warning = new List<WarningModel>();

                                foreach (var warning in article.data.warning)
                                {
                                    var warn = new WarningModel()
                                    {
                                        Message = TextContentHelper.CreateTextContent(_context, warning, lang)
                                    };
                                    _context.Warnings.Add(warn);    
                                    _context.SaveChanges() ;
                                    new_article.Warning.Add(warn);
                                }


                            }

                            if(article.data.videos != null && article.data.videos.Count != 0)
                            {
                                foreach(var videos in article.data.videos)
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
                                    _context.SaveChanges(); 

                                    new_article.Video.Add(new_video) ;
                                }
                            }

                            if(article.data.images != null && article.data.images.Count != 0)
                            {
                                foreach(var images in article.data.images)
                                {
                                    var new_image = new ImageModel()
                                    {

                                    };
                                }

                            }  



                        }

                    }


                }
            }



            //string artId = "334251100";
            //string endpoint = "https://rozetka.com.ua/api/product-api/v4/goods/get-main?front-type=xl&country=UA&lang=ru&goodsId=" + artId;
            //var client = new HttpClient();
            //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, endpoint);

            //var response = await client.SendAsync(request);
            //var responseBody = await response.Content.ReadAsStringAsync();

            //var myDeserializedClass = JsonConvert.DeserializeObject<DiplomaMarketBackend.Parser.Article.Root>(responseBody);

            return Ok($"Received json");
        }

        [HttpPost]
        [Route("getCategories")]
        public async Task<IActionResult> GetCategories([FromQuery]string lang)
        {
            var top_categories = _context.Categories
                .Include(c=>c.Name)
                .ThenInclude(n => n.Translations)
                .Where(c=>c.ParentCategoryId == null)
                .ToList();

            var result = new List<Models.Category>();

            foreach (var category in top_categories)
            {
                var output_cat = new Models.Category();

                if(category.Name != null)
                {
                    output_cat.Name = category.Name.Translations.FirstOrDefault(t => t.LanguageId == lang.ToUpper()) != null ?
                    category.Name.Translations.First(t => t.LanguageId == lang.ToUpper()).TranslationString :
                    category.Name.OriginalText;
                }

                var mid_childs = _context.Categories.Include(c => c.Name).ThenInclude(n=>n.Translations).Where(c => c.ParentCategoryId == category.Id).ToList();

                foreach(var child in mid_childs)
                {
                    var output_mchild = new Models.Category();
                    if(child.Name != null)
                    {
                        output_mchild.Name = child.Name.Translations.FirstOrDefault(t => t.LanguageId == lang.ToUpper()) != null ?
                        child.Name.Translations.FirstOrDefault(t => t.LanguageId == lang.ToUpper()).TranslationString :
                        child.Name.OriginalText;
                    }

                    var low_childs =  _context.Categories.Include(c => c.Name).ThenInclude(n => n.Translations).Where(c => c.ParentCategoryId == child.Id).ToList();

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

            var client = new MongoClient("mongodb://root:example@localhost:27017");
            var db = client.GetDatabase("test");
            IGridFSBucket gridFS = new GridFSBucket(db);
            ObjectId id;
            using (var webclient = new WebClient())
            {
                var content = webclient.DownloadData($"https://content.rozetka.com.ua/goods/images/preview/251021268.jpg");
                using (var stream = new MemoryStream(content))
                {
                   id  = await gridFS.UploadFromStreamAsync("somename", stream);
                }
            }

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
            var bytes = _fileService.GetFile(img_id,"fs").Result;
            return File(bytes,MimeType);
        }

    }
}
