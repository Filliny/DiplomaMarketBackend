using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Models;
using DiplomaMarketBackend.Helpers;
using DiplomaMarketBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomaMarketBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        ILogger<ValuesController> _logger;
        BaseContext _context;

        public ValuesController(ILogger<ValuesController> logger, BaseContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Get category-related characteristics
        /// </summary>
        /// <param name="category_id">Category id</param>
        /// <param name="search">search string from drop-down</param>
        /// <param name="lang">language</param>
        /// <returns>List of characteristics</returns>
        [HttpGet]
        [Route("category-characteristics")]
        public async Task<IActionResult> GetCategoryCharacteristics([FromQuery] int category_id, string? search, string lang)
        {
            lang = lang.NormalizeLang();

            var charakteristics = await _context.ArticleCharacteristics.Include(c => c.Title.Translations).Where(c => c.CategoryId == category_id).ToListAsync();

            if (search != null)
            {
                charakteristics = charakteristics.Where(c => c.Title.Translations.Any(t => t.TranslationString.ToUpper().Contains(search.ToUpper()) && t.LanguageId == lang)).ToList();
            }

            var result = new List<dynamic>();

            foreach (var charakter in charakteristics)
            {
                result.Add(new
                {
                    id = charakter.Id,
                    name = charakter.Title.Content(lang)

                });
            }

            return new JsonResult(new { data = result });

        }


        /// <summary>
        /// Get category-related characteristics with values for filter display
        /// </summary>
        /// <param name="category_id">Category id</param>
        /// <param name="lang">language</param>
        /// <returns>List of characteristics</returns>
        [HttpGet]
        [Route("category-chars-values")]
        [ResponseCache(VaryByQueryKeys = new[] { "category_id", "lang" }, Duration = 3600)]
        public async Task<IActionResult> GetCategoryCharacteristics([FromQuery] int category_id, string lang)
        {
            lang = lang.NormalizeLang();

            var chars = await  _context.Articles.AsSplitQuery().
                Include(a=>a.CharacteristicValues).ThenInclude(v=>v.CharacteristicType.Title.Translations).
                Include(a => a.CharacteristicValues).ThenInclude(v => v.Title.Translations)
                .Where(a=>a.CategoryId == category_id).ToListAsync();

            var groups = chars.SelectMany(a => a.CharacteristicValues).Distinct().GroupBy(v=>v.CharacteristicType).ToList();

            var result = new List<dynamic>();

            foreach (var charakter in groups)
            {
                if(!charakter.Key.show_in_filter) continue;

                int min = charakter.Key.filterType == FilterType.slider ? int.MaxValue : 0;
                int max = 0;
                var values = new List<dynamic>();

                foreach (var value in charakter)
                {
                    if (charakter.Key.filterType == FilterType.slider)
                    {
                        if (int.TryParse(value.Title.OriginalText, out int val))
                        {
                            if (val < min) min = val;
                            if (val > max) max = val;
                        }
                    }

                    values.Add(new
                    {
                        id = value.Id,
                        name = value.Title.Content(lang)
                    });
                }

                var charakt = new
                {
                    id = charakter.Key.Id,
                    name = charakter.Key.Title.Content(lang),
                    filter_type = charakter.Key.filterType.ToString(),
                    lower_value = min,
                    upper_value = max,
                    values

                };

                result.Add(charakt);
            }

            return new JsonResult(new { data = result });
        }

        /// <summary>
        /// Get characteristic by id 
        /// </summary>
        /// <param name="id">characteristic id</param>
        /// <returns>One characteristic with values by id for editing</returns>
        /// <response code="404">If the characteristic is not found</response>
        [HttpGet]
        [Route("charakteristic")]
        public async Task<ActionResult<Characteristic>> GetCharakterisitic([FromQuery] int id)
        {
            var characteristic = await _context.CharacteristicValues.
                Include(v => v.Title.Translations).
                Include(v => v.CharacteristicType).ThenInclude(t => t.Title.Translations).
                Include(v => v.CharacteristicType.Group.groupTitle.Translations).
                Where(v => v.CharacteristicTypeId == id).GroupBy(v => v.CharacteristicType).ToListAsync();

            if (characteristic == null) return StatusCode(StatusCodes.Status404NotFound, new Result()
            {
                Status = "Error",
                Message = "No such characteristic"
            });

            if (characteristic != null && characteristic.Count != 0)
            {

                var result = new Characteristic()
                {
                    id = characteristic[0].Key.Id,
                    is_active = characteristic[0].Key.Status == "active",
                    category_id = characteristic[0].Key.CategoryId,
                    show_order = characteristic[0].Key.Order,
                    comparable = characteristic[0].Key.Comparable,
                    filter_type = characteristic[0].Key.filterType.ToString(),
                    show_in_filter = characteristic[0].Key.show_in_filter
                };

                foreach (var name in characteristic[0].Key.Title.Translations)
                {
                    result.Names.Add(name.LanguageId, name.TranslationString);
                }

                if (characteristic[0].Key.Group != null)
                {

                    result.group = new Characteristic.Group() { id = characteristic[0].Key.Group.Id, show_order = characteristic[0].Key.Group.group_order };

                    foreach (var transl in characteristic[0].Key.Group.groupTitle.Translations)
                    {
                        result.group.Translations.Add(transl.LanguageId, transl.TranslationString);
                    }

                }

                foreach (var group in characteristic)
                {
                    foreach (var value in group)
                    {
                        var add_val = new Characteristic.Value() { id = value.Id };

                        foreach (var transl in value.Title.Translations)
                        {
                            add_val.Translations.Add(transl.LanguageId, transl.TranslationString);
                        }

                        result.Values.Add(add_val);
                    }
                }

                return new JsonResult(result);
            }

            return NotFound("No such characteristic!");

        }


        /// <summary>
        /// Get only values list by characteristic id 
        /// </summary>
        /// <param name="id">characteristic id</param>
        /// <param name="lang">language</param>
        /// <returns>One characteristic with values by id for editing</returns>
        /// <response code="404">If the characteristic is not found</response>
        [HttpGet]
        [Route("characteristic-values")]
        public async Task<ActionResult<Characteristic>> GetValues([FromQuery] int id, string lang)
        {
            lang = lang.NormalizeLang();

            var characteristic = await _context.CharacteristicValues.
                Include(v => v.Title.Translations).
                Where(v => v.CharacteristicTypeId == id).ToListAsync();

            if (characteristic == null) return StatusCode(StatusCodes.Status404NotFound, new Result()
            {
                Status = "Error",
                Message = "No such characteristic"
            });

            var result = new List<dynamic>();

            if (characteristic != null && characteristic.Count != 0)
            {
                foreach (var value in characteristic)
                {
                    result.Add(new
                    {
                        id = value.Id,
                        value = value.Title.Content(lang)
                    });
                }

                return new JsonResult(new { data = result });
            }

            return NotFound("No such characteristic!");
        }

        /// <summary>
        /// Creates new characteristic from json body entity
        /// </summary>
        /// <remarks>
        ///  - Names and Translations must be dictionary with LanguageId key and Translation string value
        ///  - Default locality key "UK" is required
        ///  - e.g. "UK":"Широкоекранний"
        /// </remarks>
        /// <param name="characteristic">Caharacterisitc entity</param>
        /// <returns>OK and Result entity with Status and Message </returns>
        /// <response code="400">If entity is wrong</response>
        /// <response code="500">If adding characteristic failed (see message)</response>
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<Result>> CreateCharacteristic([FromBody] Characteristic characteristic)
        {
            var new_characteristic = new ArticleCharacteristic();
            var op_values = new List<ValueModel>();

            try
            {
                if (!characteristic.Names.ContainsKey("UK")) return BadRequest (new Result()
                {
                    Status = "Error",
                    Message = "Name lacks default translation string locale UK",
                    Entity = characteristic,

                });

                new_characteristic.Title = TextContentHelper.CreateFromDictionary(_context, characteristic.Names);
                new_characteristic.Name = new_characteristic.Title;
                new_characteristic.Status = characteristic.is_active ? "active" : "inactive";
                new_characteristic.CategoryId = characteristic.category_id;
                new_characteristic.Order = characteristic.show_order;
                new_characteristic.Comparable = characteristic.comparable;
                new_characteristic.filterType = Enum.TryParse(typeof(FilterType), characteristic.filter_type, out object? result) ? (FilterType)result : FilterType.checkbox;
                new_characteristic.show_in_filter = characteristic.show_in_filter;

                foreach (var value in characteristic.Values)
                {
                    var new_value = new ValueModel()
                    {
                        Title = TextContentHelper.CreateFromDictionary(_context, value.Translations),
                    };

                    op_values.Add(new_value);
                    new_characteristic.Values.Add(new_value);
                }

                if (characteristic.group != null)
                {
                    if (characteristic.group.id != 0) new_characteristic.GroupId = characteristic.group.id;
                    //todo group translations parsing if have changes
                    else
                    {
                        var new_group = new CharacteristicGroupModel()
                        {
                            groupTitle = TextContentHelper.CreateFromDictionary(_context, characteristic.group.Translations),
                            group_order = characteristic.group.show_order
                        };

                        //_context.CharacteristicGroups.Add(new_group);
                        new_characteristic.Group = new_group;
                    }

                }

                _context.ArticleCharacteristics.Add(new_characteristic);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                TextContentHelper.Delete(_context, new_characteristic.Name);
                foreach (var value in op_values)
                {
                    TextContentHelper.Delete(_context, value.Title);
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = "Error",
                    ex.Message
                });

            }


            return StatusCode(StatusCodes.Status200OK, new Result()
            {
                Status = "Success",
                Message = "Characteristic created sucessfully!",
                Entity = characteristic
            });
        }

        /// <summary>
        /// Updates new characteristic from json body entity
        /// </summary>
        /// <remarks>
        ///  - Names and Translations must be dictionary with LanguageId key and Translation string value
        ///  - Default locality key "UK" is required
        ///  - e.g. "UK":"Широкоекранний"
        /// </remarks>
        /// New characteristic values and characterisitc group id's must be =0 or absent
        /// <param name="characteristic">Caharacterisitc entity</param>
        /// <returns>OK and Result entity with Status and Message </returns>
        /// <response code="400">If entity is wrong</response>
        /// <response code="500">If adding characteristic failed (see message)</response>
        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> UpdateCharacteristic([FromBody] Characteristic characteristic)
        {
            var exist_chr = await _context.ArticleCharacteristics.
                Include(c => c.Title.Translations).
                Include(c => c.Values).ThenInclude(v => v.Title.Translations).
                Include(c => c.Group.groupTitle.Translations).
                FirstOrDefaultAsync(c => c.Id == characteristic.id);

            if (characteristic.id == 0 || exist_chr == null) return StatusCode(StatusCodes.Status400BadRequest, new Result()
            {
                Status = "Error",
                Message = "No such characteristic!",
                Entity = characteristic
            });

            exist_chr.Status = characteristic.is_active ? "active" : "inactive";
            exist_chr.CategoryId = characteristic.category_id;
            exist_chr.Order = characteristic.show_order;
            exist_chr.Comparable = characteristic.comparable;
            exist_chr.filterType = Enum.TryParse(typeof(FilterType), characteristic.filter_type, out object? result) ? (FilterType)result : FilterType.checkbox;
            exist_chr.show_in_filter = characteristic.show_in_filter;

            try
            {
                foreach (var name in characteristic.Names)
                {

                    TextContentHelper.UpdateTextContent(_context, name.Value, exist_chr.TitleId, name.Key);

                }

                foreach (var value in characteristic.Values)
                {
                    if (value.id != 0)
                    {

                        var exst_val = exist_chr.Values.FirstOrDefault(v => v.Id == value.id);

                        if (exst_val != null)
                        {

                            foreach (var trans in value.Translations)
                            {
                                TextContentHelper.UpdateTextContent(_context, trans.Value, exst_val.TitleId, trans.Key);
                            }
                            _context.CharacteristicValues.Update(exst_val);
                        }
                        else
                            throw new Exception($"Value id:{value.id} not found for characteristic id:{exist_chr.Id} ");
                    }
                    else
                    {
                        var new_val = new ValueModel()
                        {
                            Title = TextContentHelper.CreateFromDictionary(_context, value.Translations)
                        };

                        exist_chr.Values.Add(new_val);
                    }

                }

                if (characteristic.group != null)
                {
                    if (characteristic.group.id != 0)
                    {
                        var group = _context.CharacteristicGroups.FirstOrDefault(g => g.Id == characteristic.group.id);
                        if (group == null) throw new Exception($"Characteruistic group id:{characteristic.group.id} not exist!");

                        foreach (var transl in characteristic.group.Translations)
                        {
                            TextContentHelper.UpdateTextContent(_context, transl.Value, group.groupTitleId, transl.Key);
                        }
                    }
                    else
                    {
                        var group = new CharacteristicGroupModel()
                        {

                            groupTitle = TextContentHelper.CreateFromDictionary(_context, characteristic.group.Translations),
                            group_order = characteristic.group.show_order
                        };

                        exist_chr.Group = group;
                    }

                }

                _context.ArticleCharacteristics.Update(exist_chr);
                _context.SaveChanges();


            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Result()
                {
                    Status = "Error",
                    Message = e.Message,
                    Entity = characteristic
                });
            }

            return StatusCode(StatusCodes.Status200OK, new Result()
            {
                Status = "Success",
                Message = "Characteristic updated sucessfully!"
            });
        }

        /// <summary>
        /// Delete characteristic by id with related values
        /// </summary>
        /// <param name="id">Characteristic Id</param>
        /// <param name="no_check">Set to True to skip related articles existence check</param>
        /// <returns>Ok of success</returns>
        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteCharacteristic([FromQuery] int id, bool no_check = false)
        {
            var characteristic = await _context.ArticleCharacteristics.FindAsync(id);

            if (characteristic == null) return StatusCode(StatusCodes.Status400BadRequest, new Result()
            {
                Status = "Error",
                Message = "Characteristic not found",

            });

            if (!no_check && _context.ArticleCharacteristics.Include(c => c.Values).ThenInclude(v => v.Articles).Any(c => c.Values.Any(v => v.Articles.Count != 0)))
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new Result()
                {
                    Status = "Error",
                    Message = "Characteristic values still related to articles",
                    Entity = new
                    {
                        articles_related = await _context.CharacteristicValues.Include(v => v.Articles).Where(v => v.CharacteristicTypeId == id).SumAsync(v => v.Articles.Count)
                    }
                });
            }

            _context.ArticleCharacteristics.Remove(characteristic);

            return StatusCode(StatusCodes.Status200OK, new Result()
            {
                Status = "Success",
                Message = "Characteristic deleted",

            });
        }
    }

}