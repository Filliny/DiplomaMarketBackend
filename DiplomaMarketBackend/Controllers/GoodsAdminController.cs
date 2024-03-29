﻿using AutoMapper;
using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Models;
using DiplomaMarketBackend.Helpers;
using DiplomaMarketBackend.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;
using MongoDB.Driver.Linq;
using System.Net;

namespace DiplomaMarketBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoodsAdminController : Controller
    {
        ILogger<GoodsAdminController> _logger;
        BaseContext _context;
        IFileService _fileService;
        IMapper _mapper;

        public GoodsAdminController(ILogger<GoodsAdminController> logger, BaseContext context, IFileService fileService, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _fileService = fileService;
            _mapper = mapper;
        }

        /// <summary>
        /// Articles list with filtering for articles and storage admin
        /// </summary>
        /// <param name="lang">Languge id</param>
        /// <param name="search">Search by article name string(can be null)</param>
        /// <param name="article_id">Search by article id(can be null)</param>
        /// <param name="category_id">Search by category id (can be null)</param>
        /// <param name="page">Page number for pagination (default = 1)</param>
        /// <param name="limit">Articles quantity for display (default =10)</param>
        /// <returns>List of for display table with preview image and quantity</returns>
        [HttpGet]
        [Route("get-list")]
        public async Task<IActionResult> GetArticlesList([FromQuery]string lang, string? search, int? article_id, int? category_id, int page=1, int limit=10 )
        {
            lang = lang.NormalizeLang();

            var articles = new List<dynamic>();

            var goods = _context.Articles.AsNoTracking().AsSplitQuery().
                Include(a => a.Title.Translations).
                Include(a => a.Category).
                Include(a=>a.Images).ThenInclude(p=>p.small).
                Include(a=>a.Category.Name.Translations).
                Select(a=>a);

            if (article_id is not null)
            {
                goods = goods.Where(a => a.Id == article_id);
            }
            else
            {
                if (category_id is not null)
                {
                    goods = goods.Where(a => a.CategoryId == category_id);
                }
                
                if (search is not null)
                {
                    goods = goods.Where(c => c.Title.Translations.Any(t => t.TranslationString.ToLower().Contains(search.ToLower()) && t.LanguageId == lang));
                }
            }

            var goodsList = await goods.ToListAsync();

            int total_goods = goodsList.Count;
            int total_pages = (int)Math.Ceiling((decimal)total_goods / (decimal)limit);
            
            if (page > total_pages) page = total_pages;

            int skip = (page - 1) * limit;

            goodsList = goodsList.Skip(skip).Take(limit).ToList();

            foreach (var article in goodsList)
            {
                articles.Add(new
                {
                    id= article.Id,
                    name = article.Title.Content(lang),
                    price = article.Price,
                    category = article.Category.Name.Content(lang),
                    preview = Request.GetImageURL(BucketNames.small.ToString(),article.Images.First().small.url??""),
                    quantity = article.Quantity,
                    status = article.Status
                });
            }

            var result = new
            {
                data = new
                {
                    articles,
                    total_goods,
                    total_pages,
                    displayed_page = page
                }
            };

            return new JsonResult(result);
            
        }

        /// <summary>
        /// Get article by id for editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Article entity for editing</returns>
        /// <response code="404">If the request article not found</response>
        [HttpGet]
        [Route("get")]
        public async Task<ActionResult<Article>> GetArticle([FromQuery] int id)
        {
            var article = await _context.Articles.AsNoTracking().AsSplitQuery().
                Include(a => a.Title.Translations).
                Include(a => a.Description.Translations).
                Include(a => a.Docket.Translations).
                Include(a => a.Warning).ThenInclude(w => w.Message.Translations).
                Include(a => a.Actions).
                Include(a => a.Video).
                Include(a => a.Images).ThenInclude(i => i.preview).
                Include(a => a.CharacteristicValues).ThenInclude(v => v.CharacteristicType).ThenInclude(v => v.Title).
                Include(a => a.CharacteristicValues).ThenInclude(v => v.Title).
                FirstOrDefaultAsync(a => a.Id == id);

            //var mappedresult = _mapper.Map<ArticleDTO>(article);
            //var arti = _mapper.Map<ArticleModel>(mappedresult);

            if (article == null) return StatusCode(StatusCodes.Status404NotFound, new Result()
            {
                Status = "Error",
                Message = "Article not found!",
            });

            var result = article.Adapt<Article>();

            result.titles = article.Title.Translations.ToDictionary(t => t.LanguageId ?? "", t => t.TranslationString ?? "");
            result.descriptions = article.Description.Translations.ToDictionary(t => t.LanguageId ?? "", t => t.TranslationString ?? "");
            result.dockets = article.Docket.Translations.ToDictionary(t => t.LanguageId ?? "", t => t.TranslationString ?? "");
            result.video_urls = article.Video.Select(a => a.URL ?? "").ToList();
            result.warnings = article.Warning.Select(w => new Article.Warning
            {
                id = w.Id,
                messages = w.Message.Translations.ToDictionary(t => t.LanguageId ?? "", t => t.TranslationString ?? "")

            }).ToList();

            result.images = article.Images.Select(i => new Article.PreviewImage() { id = i.Id, url = Request.GetImageURL(BucketNames.preview, i.preview.url) }).ToList();

            result.values = article.CharacteristicValues.Select(v => new Article.TypeValue
            {
                value_id = v.Id,
                value_name = v.Title.OriginalText ?? "",

                charcteristic_id = v.CharacteristicTypeId,
                charcteristic_name = v.CharacteristicType.Title.OriginalText ?? ""

            }).ToList();

            result.actions_ids = article.Actions.Select(a => a.Id).ToList();
            var jsn = JsonConvert.SerializeObject(result);
            var new_article = JsonConvert.DeserializeObject<Article>(jsn);

            return new JsonResult(new { data = result, stringed = jsn });

        }

        /// <summary>
        /// Create new article
        /// </summary>
        /// <param name="article_pack">Article pack: entity article stringified + FormFiles</param>
        /// <returns>Created if success</returns>
        /// <response code="201">If article sucesfull created</response>
        /// <response code="400">If the request values is bad</response>
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<Article>> CreateArticle([FromForm] ArticlePack article_pack)
        {
            var json = Regex.Unescape(article_pack.article_json);
            Article? new_article;

            try
            {
                new_article = JsonConvert.DeserializeObject<Article>(article_pack.article_json);

                if (new_article == null) throw new Exception("Deserialize result is null");

                var new_entry = new_article.Adapt<ArticleModel>();

                new_entry.Title = TextContentHelper.CreateFromDictionary(_context, new_article.titles, false);
                new_entry.Description = TextContentHelper.CreateFromDictionary(_context, new_article.descriptions, false);
                new_entry.Docket = TextContentHelper.CreateFromDictionary(_context, new_article.dockets, false);
                new_entry.Video = new_article.video_urls.Select(v => new VideoModel(v)).ToList();
                new_entry.TopCategoryId = await CatHelper.GetTopCat(_context, new_article.category_id);

                if(new_article.points != 0)
                {
                    var bonuses_char = _context.CharacteristicValues.Include(c => c.Title).FirstOrDefault(c => c.Title.OriginalText.Equals("З бонусами"));
                    if(bonuses_char != null) { new_entry.CharacteristicValues.Add(bonuses_char); }
                }
                
                if(new_article.warnings is not null)
                    new_entry.Warning = new_article.warnings.Select(w => new WarningModel()
                    {
                        Message = TextContentHelper.CreateFromDictionary(_context, w.messages, false)
                    }).ToList();

                foreach (var value in new_article.values)
                {
                    if(value.value_id == 0 )
                    {
                        if(value.charcteristic_id != 0)
                        {
                            var new_value = new ValueModel
                            {
                                //todo auto add translations 
                                Title = new TextContent { OriginalText = value.value_name, OriginalLanguageId = "UK" },
                                CharacteristicTypeId = value.charcteristic_id,
                            };
                            new_entry.CharacteristicValues.Add(new_value);
                        }
                    }
                    else
                    {
                        var ex_value = _context.CharacteristicValues.FirstOrDefault(v => v.Id == value.value_id);
                        if (ex_value != null)
                            new_entry.CharacteristicValues.Add(ex_value);
                    }

                }

                foreach (var action_id in new_article.actions_ids)
                {
                    var action = _context.Actions.FirstOrDefault(v => v.Id == action_id);
                    if (action != null)
                        new_entry.Actions.Add(action);
                }

                new_entry.Images.Clear();

                foreach (var image in article_pack.images)
                {
                    new_entry.Images.Add(await GetImagePictires(image.OpenReadStream(), image.FileName));
                }

                new_entry.Created = DateTime.Now;
                new_entry.Id = 0; //ensure for test

                //process category-brands relation
                var category = await _context.Categories.Include(c => c.Brands).FirstOrDefaultAsync(c => c.Id == new_article.category_id);
                if (category != null)
                {
                    if (!category.Brands.Any(b => b.Id == new_article.brand_id))
                    {
                        var brand = await _context.Brands.FindAsync(new_article.brand_id);
                        if (brand != null) category.Brands.Add(brand);
                        //_context.SaveChanges();
                    }
                }

                _context.Articles.Add(new_entry);
                _context.SaveChanges();
                new_article.id = new_entry.Id;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Result
                {
                    Status = "Error",
                    Message = "Error create article :" + ex.Message,
                });
            }

            return StatusCode(StatusCodes.Status201Created, new Result
            {
                Status = "Success",
                Message = "Article created",
                Entity = new_article
            });
        }


        /// <summary>
        /// create article from simple json with images from outer url
        /// </summary>
        /// <param name="new_article"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Route("create-json")]
        //public async Task<ActionResult<Article>> CreateJsonArticle([FromBody] Article new_article)
        //{
        //     try{


        //        if (new_article == null) throw new Exception("Deserialize result is null");

        //        var new_entry = new_article.Adapt<ArticleModel>();

        //        new_entry.Title = TextContentHelper.CreateFromDictionary(_context, new_article.titles, false);
        //        new_entry.Description = TextContentHelper.CreateFromDictionary(_context, new_article.descriptions, false);
        //        new_entry.Docket = TextContentHelper.CreateFromDictionary(_context, new_article.dockets, false);
        //        new_entry.Video = new_article.video_urls.Select(v => new VideoModel(v)).ToList();
        //        new_entry.TopCategoryId = await CatHelper.GetTopCat(_context, new_article.category_id);

        //        if (new_article.points != 0)
        //        {
        //            var bonuses_char = _context.CharacteristicValues.Include(c => c.Title).FirstOrDefault(c => c.Title.OriginalText.Equals("З бонусами"));
        //            if (bonuses_char != null) { new_entry.CharacteristicValues.Add(bonuses_char); }
        //        }

        //        if (new_article.warnings is not null)
        //            new_entry.Warning = new_article.warnings.Select(w => new WarningModel()
        //            {
        //                Message = TextContentHelper.CreateFromDictionary(_context, w.messages, false)
        //            }).ToList();

        //        foreach (var value in new_article.values)
        //        {
        //            if (value.value_id == 0)
        //            {
        //                if (value.charcteristic_id != 0)
        //                {
        //                    var new_value = new ValueModel
        //                    {
        //                        //todo auto add translations 
        //                        Title = new TextContent { OriginalText = value.value_name, OriginalLanguageId = "UK" },
        //                        CharacteristicTypeId = value.charcteristic_id,
        //                    };
        //                    new_entry.CharacteristicValues.Add(new_value);
        //                }
        //            }
        //            else
        //            {
        //                var ex_value = _context.CharacteristicValues.FirstOrDefault(v => v.Id == value.value_id);
        //                if (ex_value != null)
        //                    new_entry.CharacteristicValues.Add(ex_value);
        //            }

        //        }

        //        foreach (var action_id in new_article.actions_ids)
        //        {
        //            var action = _context.Actions.FirstOrDefault(v => v.Id == action_id);
        //            if (action != null)
        //                new_entry.Actions.Add(action);
        //        }

        //        new_entry.Images.Clear();

        //        foreach (var image in new_article.images)
        //        {
        //            using (var webclient = new WebClient())
        //            {
        //                var content = webclient.DownloadData(image.url);
        //                using (var stream = new MemoryStream(content))
        //                {
        //                    new_entry.Images.Add(await GetImagePictires(stream, "certificate_" + new_entry.Price.ToString()));
        //                }
        //            }
        //        }

        //        new_entry.Created = DateTime.Now;
        //        new_entry.Id = 0; //ensure for test

        //        //process category-brands relation
        //        var category = await _context.Categories.Include(c => c.Brands).FirstOrDefaultAsync(c => c.Id == new_article.category_id);
        //        if (category != null)
        //        {
        //            if (!category.Brands.Any(b => b.Id == new_article.brand_id))
        //            {
        //                var brand = await _context.Brands.FindAsync(new_article.brand_id);
        //                if (brand != null) category.Brands.Add(brand);
        //                //_context.SaveChanges();
        //            }
        //        }

        //        _context.Articles.Add(new_entry);
        //        _context.SaveChanges();
        //        new_article.id = new_entry.Id;
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status400BadRequest, new Result
        //        {
        //            Status = "Error",
        //            Message = "Error create article :" + ex.Message,
        //        });
        //    }

        //    return StatusCode(StatusCodes.Status201Created, new Result
        //    {
        //        Status = "Success",
        //        Message = "Article created",
        //        Entity = new_article
        //    });
        //}


        /// <summary>
        /// Update existing article
        /// </summary>
        /// <param name="article_pack">Article pack: entity article stringified + FormFiles</param>
        /// <returns>Accepted if succes</returns>
        /// <response code="202">If the request successfull and article updated</response>
        /// <response code="400">If the request values is bad</response>
        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> UpdateArticle([FromForm] ArticlePack article_pack)
        {
            ArticleModel? articleToUpdate = null;
            try
            {
                //var json = Regex.Unescape(article_pack.article_json);

                var updateData = JsonConvert.DeserializeObject<Article>(article_pack.article_json);
                if (updateData == null) throw new Exception("Deserialize result is null");

                articleToUpdate = await _context.Articles.AsSplitQuery().
                Include(a => a.Title.Translations).
                Include(a => a.Description.Translations).
                Include(a => a.Docket.Translations).
                Include(a => a.Warning).ThenInclude(w => w.Message.Translations).
                Include(a => a.Actions).
                Include(a => a.Video).
                Include(a=>a.Brand).
                Include(a=>a.Category).ThenInclude(c=>c.Brands).
                Include(a => a.Images).ThenInclude(i => i.preview).
                Include(a => a.CharacteristicValues).ThenInclude(v => v.CharacteristicType).ThenInclude(v => v.Title).
                Include(a => a.CharacteristicValues).ThenInclude(v => v.Title).
                FirstOrDefaultAsync(a => a.Id == updateData.id);

                if (articleToUpdate == null) throw new Exception($"Article id{updateData.id} not found!");

                var old_category = articleToUpdate.Category;
                var old_brand = articleToUpdate.Brand;

                articleToUpdate.CategoryId = updateData.category_id;
                articleToUpdate.Price = updateData.price;
                articleToUpdate.OldPrice = updateData.old_price;
                articleToUpdate.Updated = DateTime.Now;
                articleToUpdate.BrandId = updateData.brand_id;
                articleToUpdate.TopCategoryId = await CatHelper.GetTopCat(_context, updateData.category_id);
                articleToUpdate.Status = updateData.status;
                articleToUpdate.SellStatus = updateData.sell_status;
                articleToUpdate.SellerId = updateData.seller_id;

                if (updateData.points != 0)
                {
                    var bonuses_char = _context.CharacteristicValues.Include(c => c.Title).FirstOrDefault(c => c.Title.OriginalText.Equals("З бонусами"));
                    if (bonuses_char != null) { articleToUpdate.CharacteristicValues.Add(bonuses_char); }
                }
                else
                {
                    var bonuses_char = _context.CharacteristicValues.Include(c => c.Title).FirstOrDefault(c => c.Title.OriginalText.Equals("З бонусами"));
                    if (bonuses_char != null) { articleToUpdate.CharacteristicValues.Remove(bonuses_char); }
                }


                TextContentHelper.UpdateFromDictionary(_context, articleToUpdate.Title, updateData.titles);
                TextContentHelper.UpdateFromDictionary(_context, articleToUpdate.Description, updateData.descriptions);
                TextContentHelper.UpdateFromDictionary(_context, articleToUpdate.Docket, updateData.dockets);

                var to_delete = articleToUpdate.Warning.Where(w => updateData.warnings.All(u => u.id != w.Id));
                foreach (var warning in to_delete)
                {
                    articleToUpdate.Warning.Remove(warning);
                    _context.Warnings.Remove(warning);
                    TextContentHelper.Delete(_context, warning.Message);
                }

                foreach (var new_warning in updateData.warnings)
                {
                    if (new_warning.id == 0)
                    {
                        articleToUpdate.Warning.Add(new WarningModel
                        {
                            Message = TextContentHelper.CreateFromDictionary(_context, new_warning.messages)
                        });
                    }
                    else
                    {
                        var ex_warning = articleToUpdate.Warning.FirstOrDefault(x => x.Id == new_warning.id);
                        if (ex_warning != null)
                        {

                            TextContentHelper.UpdateFromDictionary(_context, ex_warning.Message, new_warning.messages);
                        }
                    }

                }

                var remove_values = articleToUpdate.CharacteristicValues.Where(w => updateData.values.All(u => u.value_id != w.Id));
                var add_values = updateData.values.Where(u => articleToUpdate.CharacteristicValues.All(a => a.Id != u.value_id));
                articleToUpdate.CharacteristicValues = articleToUpdate.CharacteristicValues.Except(remove_values).ToList();
                foreach (var value in add_values)
                {
                    var val = await _context.CharacteristicValues.FindAsync(value.value_id);
                    if (val != null) { articleToUpdate.CharacteristicValues.Add(val); }
                }

                var remove_actions = articleToUpdate.Actions.Where(w => updateData.actions_ids.All(u => u != w.Id)).ToList();
                var add_actions = updateData.actions_ids.Where(u => articleToUpdate.CharacteristicValues.All(a => a.Id != u));
                articleToUpdate.Actions = articleToUpdate.Actions.Except(remove_actions).ToList();
                foreach (var value in add_values)
                {
                    var val = await _context.Actions.FindAsync(value.value_id);
                    if (val != null) { articleToUpdate.Actions.Add(val); }
                }

                var remove_videos = articleToUpdate.Video.Where(v => updateData.video_urls.All(url => !url.Equals(v.URL)));
                var add_videos = updateData.video_urls.Where(url => articleToUpdate.Video.All(a => !a.URL.Equals(url)));
                articleToUpdate.Video = articleToUpdate.Video.Except(remove_videos).ToList();
                foreach (var video in add_videos)
                {
                    articleToUpdate.Video.Add(new VideoModel(video));
                }

                var remove_images = articleToUpdate.Images.Where(i => updateData.images.All(u => u.id != i.Id));
                articleToUpdate.Images = articleToUpdate.Images.Except(remove_images).ToList();

                //todo remove images process
                foreach (var image in article_pack.images)
                {
                    var img = await GetImagePictires(image.OpenReadStream(), image.FileName);
                    articleToUpdate.Images.Add(img);
                }

                _context.SaveChanges();

                //process category-brands relation
                if(old_brand != null && old_category.Id != updateData.category_id || old_brand.Id != updateData.brand_id)
                {
                    //if no articles with this brand left in old category remove relation
                    if (!_context.Articles.Any(a => a.CategoryId == old_category.Id && a.BrandId == old_brand.Id) && old_brand != null)
                        old_category.Brands.Remove(old_brand);

                    var new_category = await _context.Categories.Include(c => c.Brands).FirstOrDefaultAsync(c => c.Id == updateData.category_id);
                    if (new_category != null)
                    {
                        if (!new_category.Brands.Any(b => b.Id == updateData.brand_id))
                        {
                            var brand = await _context.Brands.FindAsync(updateData.brand_id);
                            if (brand != null) new_category.Brands.Add(brand);
                            _context.SaveChanges();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Result
                {
                    Status = "Error",
                    Message = "Error update article :" + ex.Message,
                });
            }

            return StatusCode(StatusCodes.Status202Accepted, new Result
            {
                Status = "Success",
                Message = "Article updated",
            });
        }


        /// <summary>
        /// Delete article and related pictures by id
        /// </summary>
        /// <param name="id">Article id</param>
        /// <returns>Ok if success</returns>
        /// <response code="500">If the request values is bad</response>
        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteArticle([FromQuery] int id)
        {
            try
            {
                if (_context.OrderItems.Any(o => o.ArticleId == id))
                    throw new Exception("Article relays with orders - cant delete! Suggest simply to disable it");

                var article = await _context.Articles.AsSplitQuery().
                Include(a => a.Title.Translations).
                Include(a => a.Description.Translations).
                Include(a => a.Docket.Translations).
                Include(a => a.Warning).ThenInclude(w => w.Message.Translations).
                Include(a => a.Video).
                Include(a=>a.Category).ThenInclude(c=>c.Brands).
                Include(a => a.Images).ThenInclude(i => i.base_action).
                Include(a => a.Images).ThenInclude(i => i.preview).
                Include(a => a.Images).ThenInclude(i => i.original).
                Include(a => a.Images).ThenInclude(i => i.large).
                Include(a => a.Images).ThenInclude(i => i.medium).
                Include(a => a.Images).ThenInclude(i => i.small).
                Include(a => a.Images).ThenInclude(i => i.big_tile).
                Include(a => a.Images).ThenInclude(i => i.big).
                Include(a => a.Images).ThenInclude(i => i.mobile_large).
                Include(a => a.Images).ThenInclude(i => i.mobile_medium).
                Include(a => a.Reviews).
                FirstOrDefaultAsync(a => a.Id == id);

                if (article == null) throw new Exception("Article not found");

                

                TextContentHelper.Delete(_context, article.Title);
                TextContentHelper.Delete(_context, article.Description);
                TextContentHelper.Delete(_context, article.Docket);
                foreach (var warning in article.Warning)
                {
                    _context.Warnings.Remove(warning);
                    TextContentHelper.Delete(_context, warning.Message);
                }

                foreach (var video in article.Video)
                {
                    _context.Videos.Remove(video);
                }

                foreach (var images in article.Images)
                {
                    _context.Pictures.Remove(images.base_action);
                    _context.Pictures.Remove(images.preview);
                    _context.Pictures.Remove(images.large);
                    _context.Pictures.Remove(images.medium);
                    _context.Pictures.Remove(images.small);
                    _context.Pictures.Remove(images.big_tile);
                    _context.Pictures.Remove(images.big);
                    _context.Pictures.Remove(images.mobile_large);
                    _context.Pictures.Remove(images.mobile_medium);

                    _fileService.DeleteFile(nameof(images.base_action), images.base_action.url);
                    _fileService.DeleteFile(nameof(images.preview), images.preview.url);
                    _fileService.DeleteFile(nameof(images.large), images.large.url);
                    _fileService.DeleteFile(nameof(images.medium), images.medium.url);
                    _fileService.DeleteFile(nameof(images.small), images.small.url);
                    _fileService.DeleteFile(nameof(images.big_tile), images.big_tile.url);
                    _fileService.DeleteFile(nameof(images.big), images.big.url);
                    _fileService.DeleteFile(nameof(images.mobile_large), images.mobile_large.url);
                    _fileService.DeleteFile(nameof(images.mobile_medium), images.mobile_medium.url);

                    _context.Images.Remove(images);


                }


                //if no articles with this brand left in old category remove relation
                if (!_context.Articles.Any(a => a.CategoryId == article.CategoryId && a.BrandId == article.BrandId) && article.Brand != null)
                    article.Category.Brands.Remove(article.Brand);

                _context.Articles.Remove(article);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Result
                {
                    Status = "Error",
                    Message = "Error delete article :" + ex.Message,
                });
            }

            return StatusCode(StatusCodes.Status200OK, new Result
            {
                Status = "Success",
                Message = "Article deleted!"
            });
        }


        [HttpPost]
        [Route("add-picture")]
        public async Task<IActionResult> AddPicture([FromForm] int id, IFormFile image)
        {
            var article = await _context.Articles.FirstOrDefaultAsync(a => a.Id == id);

            if (article == null) { return StatusCode(StatusCodes.Status404NotFound, "Article not found"); }

            var pictires = await GetImagePictires(image.OpenReadStream(),image.FileName);

            if (pictires != null)
            {
                article.Images.Add(pictires);
            }

            _context.Articles.Update(article);
            _context.SaveChanges();

            return Ok();
        }


        private async Task<ImageModel> GetImagePictires(Stream file_stream, string filename)
        {
            var image = new ImageModel();

            PropertyInfo[] pictures_prop = typeof(ImageModel).
                GetProperties(BindingFlags.Public | BindingFlags.Instance).
                Where(p => p.PropertyType == typeof(PictureModel)).ToArray();

            foreach (var prop in pictures_prop)
            {
                using var picture = Image.Load(file_stream);
                var name = prop.Name;
                var width = (int)Enum.Parse(typeof(BucketNames), name);

                if (!name.Equals("original"))
                    picture.Mutate(x => x.Resize(width, 0));

                using var stream = new MemoryStream();
                await picture.SaveAsync(stream, new JpegEncoder());

                stream.Position = 0;

                var link = await _fileService.SaveFileFromStream(name, filename, stream);


                prop.SetValue(image, new PictureModel { url = link, width = picture.Width, height = picture.Height });
                file_stream.Position = 0;
            }

            return image;
        }



        [HttpPatch]
        [Route("add-bonuses")]
        [Authorize(Roles = "LoyalityWrite")]
        public async Task<IActionResult> AddBonuses([FromForm] int article_id, uint bonuses_count)
        {
            var article = await _context.Articles.FindAsync(article_id);

            if (article == null) return BadRequest(new Result
            {
                Status = "Error",
                Message = "Article not found!",
            });

            article.Points = bonuses_count;

            var bonuses_char = _context.CharacteristicValues.Include(c => c.Title).FirstOrDefault(c => c.Title.OriginalText.Equals("З бонусами"));
            if (bonuses_char != null) { article.CharacteristicValues.Add(bonuses_char); }

            return Ok(new Result
            {
                Status = "Success",
                Message = "Bonuses added",
            });
        }

        [HttpPatch]
        [Route("remove-bonuses")]
        [Authorize(Roles ="LoyalityWrite")]
        public async Task<IActionResult> RemoveBonuses([FromForm] int article_id)
        {
            var article = await _context.Articles.FindAsync(article_id);

            if (article == null) return BadRequest(new Result
            {
                Status = "Error",
                Message = "Article not found!",
            });

            article.Points = 0;

            var bonuses_char = _context.CharacteristicValues.Include(c => c.Title).FirstOrDefault(c => c.Title.OriginalText.Equals("З бонусами"));
            if (bonuses_char != null) { article.CharacteristicValues.Remove(bonuses_char); }

            return Ok(new Result
            {
                Status = "Success",
                Message = "Bonuses removed",
            });

        }
    }
}
