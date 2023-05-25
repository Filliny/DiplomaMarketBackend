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
            var settings = new FixedFilterSettingsModel();

            if(category_id != 0) settings = _context.FixedFilterSettings.Include(c=>c.Category.Name).FirstOrDefault(s=>s.CategoryId == category_id)??settings;

            var common = await _context.ArticleCharacteristics.AsSplitQuery().
                    Include(c => c.Title.Translations).
                    Include(c => c.Values).ThenInclude(v => v.Title.Translations).
                    Where(c => c.CategoryId == null).ToListAsync();





            return Ok();
        }

        /// <summary>
        /// Create settings from given values
        /// </summary>
        /// <param name="filter">Filtes data with trasnslations added (u can send only changed data)</param>
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
            if (_context.FixedFilterSettings.Any(f => f.Id == filter.category_id)) return BadRequest(new Result
            {
                Status = "Error",
                Message = "Settings for this category already exist",
                Entity = filter
            });
            var settings = filter.Adapt<FixedFilterSettingsModel>();

          

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




     
    }
}
