using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Models;
using DiplomaMarketBackend.Helpers;
using DiplomaMarketBackend.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomaMarketBackend.Controllers
{
    public class FiltersAdminController : Controller
    {
        ILogger<GoodsController> _logger;
        BaseContext _context;
        IFileService _fileService;

        public FiltersAdminController(ILogger<GoodsController> logger, BaseContext context, IFileService fileService)
        {
            _logger = logger;
            _context = context;
            _fileService = fileService;
        }

        /// <summary>
        /// Get list of categories that have settings added
        /// </summary>
        /// <returns>List of categories with id and name</returns>
        [HttpGet]
        [Route("get-list")]
        public async Task<IActionResult> GetSettedList()
        {
            var result = await _context.FixedFilterSettings.
                Include(f=>f.Category.Name).
                Select(f=>new {id= f.Id, name = f.Category.Name.OriginalText}).ToListAsync();

            return Json(result);
        }

        /// <summary>
        /// Get list of categories that havent settings added-( for new setting in admin inteface)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-candidates")]
        public async Task<IActionResult> GetCandidatesList()
        {
            var existing  = await _context.FixedFilterSettings.Include(f => f.Category.Name).Select(f =>  f.CategoryId).ToListAsync();
            var categoryes = await _context.Categories.
                 Include(c => c.Name).
                 Where(c=> !existing.Contains(c.Id)).
                 Select(c => new { id = c.Id, name = c.Name.OriginalText }).ToListAsync();

            return Json(categoryes);
        }

        /// <summary>
        /// Get settings for given category id
        /// </summary>
        /// <param name="category_id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-settings")]
        public async Task<ActionResult<FixedFilter>> GetFilterSettings([FromQuery] int category_id = 0)
        {
            var settings = _context.FixedFilterSettings.Include(c=>c.Category.Name).FirstOrDefault(s=>s.CategoryId == category_id)?? new FixedFilterSettingsModel(); ;

            var common_filters = await _context.ArticleCharacteristics.AsSplitQuery().
                    Include(c => c.Title.Translations).
                    Include(c => c.Values).ThenInclude(v => v.Title.Translations).
                    Where(c => c.CategoryId == null).ToListAsync();

            var response = settings.Adapt<FixedFilter>();

            foreach (var filter in common_filters) {

                var character_dto = new FixedFilter.CharacteristicType
                {
                    id = filter.Id,
                    characteristic_names = filter.Title.Translations.ToDictionary(t => t.LanguageId ?? "", t => t.TranslationString ?? ""),
                }; 

                foreach(var value in filter.Values) {

                    character_dto.values.Add(new FixedFilter.ValueType
                    {
                        id = value.Id,
                        value_names = value.Title.Translations.ToDictionary(t => t.LanguageId ?? "", t => t.TranslationString ?? "")
                    });
                }

                response.characteristics.Add(character_dto);
            }

            return Json(response);
        }

        /// <summary>
        /// Create settings from given values
        /// </summary>
        /// <param name="filter">Filtes data with trasnslations added (u can send only changed data or only settings)</param>
        /// <returns>Ok if success</returns>
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<FixedFilter>> CreateSetting([FromBody] FixedFilter filter)
        {
            if (_context.FixedFilterSettings.Any(f => f.Id == filter.category_id)) return BadRequest(new Result
            {
                Status = "Error",
                Message = "Settings for this category already exist",
                Entity = filter
            });

            var  settings = filter.Adapt<FixedFilterSettingsModel>();

            //update strings if have sended
            if(filter.characteristics != null && filter.characteristics.Count > 0)
            {
                var common_filters = await _context.ArticleCharacteristics.AsSplitQuery().
                    Include(c => c.Title.Translations).
                    Include(c => c.Values).ThenInclude(v => v.Title.Translations).
                    Where(c => c.CategoryId == null).ToListAsync();

                foreach (var characteristic in filter.characteristics)
                {
                    var ex_charact = common_filters.FirstOrDefault(f => f.Id == characteristic.id);
                    if (ex_charact != null)
                        TextContentHelper.UpdateFromDictionary(_context, ex_charact.Title, characteristic.characteristic_names, false);

                    foreach(var value in  characteristic.values)
                    {
                        var ex_value = ex_charact.Values.FirstOrDefault(v=>v.Id == value.id);
                        if(ex_value != null)
                            TextContentHelper.UpdateFromDictionary(_context,ex_value.Title,value.value_names, false);
                    }
                }   

            }

            _context.FixedFilterSettings.Add(settings);
            _context.SaveChanges();

            return Ok(new Result
            {
                Status = "Success",
                Message = "Settings added sucessfullty",
                Entity = filter
            });

        }

        /// <summary>
        /// Update setting by given values
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("update")]
        public async Task<ActionResult<FixedFilter>> UpdateSetting([FromBody] FixedFilter filter)
        {
            if (!_context.FixedFilterSettings.Any(f => f.Id == filter.category_id)) return BadRequest(new Result
            {
                Status = "Error",
                Message = "Settings for this category not exist",
                Entity = filter
            });

            var settings = filter.Adapt<FixedFilterSettingsModel>();


            //update strings if have sended
            if (filter.characteristics != null && filter.characteristics.Count > 0)
            {
                var common_filters = await _context.ArticleCharacteristics.AsSplitQuery().
                    Include(c => c.Title.Translations).
                    Include(c => c.Values).ThenInclude(v => v.Title.Translations).
                    Where(c => c.CategoryId == null).ToListAsync();

                foreach (var characteristic in filter.characteristics)
                {
                    var ex_charact = common_filters.FirstOrDefault(f => f.Id == characteristic.id);
                    if (ex_charact != null)
                        TextContentHelper.UpdateFromDictionary(_context, ex_charact.Title, characteristic.characteristic_names, false);

                    foreach (var value in characteristic.values)
                    {
                        var ex_value = ex_charact.Values.FirstOrDefault(v => v.Id == value.id);
                        if (ex_value != null)
                            TextContentHelper.UpdateFromDictionary(_context, ex_value.Title, value.value_names, false);
                    }
                }

            }

            _context.FixedFilterSettings.Update(settings);
            _context.SaveChanges();

            return Ok(new Result
            {
                Status = "Success",
                Message = "Settings added sucessfullty",
                Entity = filter
            });

        }

        /// <summary>
        /// Remove settings by given id
        /// </summary>
        /// <param name="setting_id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("delete")]
        public async Task<ActionResult<FixedFilter>> DeleteSetting([FromQuery] int setting_id)
        {
            if (!_context.FixedFilterSettings.Any(f => f.Id == setting_id)) return BadRequest(new Result
            {
                Status = "Error",
                Message = "Settings not found",
               
            });
            var settings = _context.FixedFilterSettings.First(f => f.Id == setting_id);

            _context.FixedFilterSettings.Remove(settings);
            _context.SaveChanges();

            return Ok(new Result
            {
                Status = "Success",
                Message = "Settings removed sucessfullty",

            });

        }

        /// <summary>
        /// Get category-related characteristics that not shown in filter currently
        /// </summary>
        /// <param name="category_id">Category id</param>
        /// <param name="search">search string from drop-down</param>
        /// <param name="lang">language</param>
        /// <returns>List of characteristics</returns>
        [HttpGet]
        [Route("get-unselected")]
        public async Task<ActionResult<FilterCharacteristics>> GetUnselected([FromQuery] int category_id, string? search, string lang)
        {
            lang = lang.NormalizeLang();

            var charakteristics = _context.ArticleCharacteristics.
                Include(c => c.Title.Translations).
                Include(c=>c.Category.Name.Translations).
                Where(c => c.CategoryId == category_id && !c.show_in_filter);

            if (search != null)
            {
                charakteristics = charakteristics.Where(c => c.Title.Translations.Any(t => t.TranslationString.ToUpper().Contains(search.ToUpper()) && t.LanguageId == lang));
            }

            var result = new FilterCharacteristics();

            foreach (var charakter in charakteristics)
            {
                result.chartacteristics.Add(new FilterCharacteristics.ListChracteristic
                {
                    id = charakter.Id,
                    name = charakter.Title.Content(lang)

                });
            }

            return new JsonResult(result);

        }

        /// <summary>
        /// Get category-related characteristics that shown in filter currently
        /// </summary>
        /// <param name="category_id">Category id</param>
        /// <param name="lang">language</param>
        /// <returns>List of characteristics showed in filter</returns>
        [HttpGet]
        [Route("get-selected")]
        public async Task<ActionResult<FilterCharacteristics>> GetSelected([FromQuery] int category_id, string lang)
        {
            lang = lang.NormalizeLang();

            var charakteristics = _context.ArticleCharacteristics.
                Include(c => c.Title.Translations).
                Include(c => c.Category.Name.Translations).
                Where(c => c.CategoryId == category_id && c.show_in_filter);

            var result = new FilterCharacteristics {

                category_id = category_id,
                category_name = charakteristics.First().Category?.Name.Content(lang)??""
            };

            foreach (var charakter in charakteristics)
            {
                result.chartacteristics.Add(new FilterCharacteristics.ListChracteristic
                {
                    id = charakter.Id,
                    name = charakter.Title?.Content(lang)??""
                });
            }

            return new JsonResult(result);
        }

        /// <summary>
        /// Update showed characteristics list
        /// </summary>
        /// <param name="update_list">Filter characteristic entity caontains only selected for showing characteristics</param>
        /// <returns>Ok if success</returns>
        [HttpPatch]
        [Route("filters-update")]
        public async Task<IActionResult> UpdateShowed([FromBody ] FilterCharacteristics update_list)
        {
            var category_caharacteristics = await _context.ArticleCharacteristics.Where(c => c.CategoryId == update_list.category_id).ToListAsync();

            foreach(var characteristic in  category_caharacteristics)
            {
                if(update_list.chartacteristics.Any(c=>c.id == characteristic.Id))
                {
                    characteristic.show_in_filter = true;
                }
                else
                {
                    characteristic.show_in_filter = false;
                }

                 _context.ArticleCharacteristics.Update(characteristic);

            }

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
