using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Models;
using DiplomaMarketBackend.Helpers;
using DiplomaMarketBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DiplomaMarketBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActionsController : ControllerBase
    {
        ILogger<GoodsController> _logger;
        BaseContext _context;
        IFileService _fileService;

        public ActionsController(ILogger<GoodsController> logger, BaseContext context, IFileService fileService)
        {
            _logger = logger;
            _context = context;
            _fileService = fileService;
        }

        /// <summary>
        /// Get list of existing actions
        /// </summary>
        /// <param name="lang">Laguage</param>
        /// <returns>List of all actions</returns>
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> ActionsList([FromQuery] string lang)
        {
            var actions = await _context.Actions.
                Include(a => a.Articles).
                Include(a => a.Name.Translations).
                Include(a => a.Description.Translations).
                Select(a => new
                {
                    id = a.Id,
                    name = a.Name.Content(lang),
                    description = a.Description.Content(lang),
                    articles_in = a.Articles.Count(),
                    status = a.Status,
                    start_date = a.StartDate,
                    end_date = a.EndDate,
                    banner_big = Request.GetImageURL(BucketNames.banner, a.BannerBig),
                    banner_small = Request.GetImageURL(BucketNames.banner, a.BannerSmall)

                }).ToListAsync();

            return Ok(actions);
        }


        /// <summary>
        /// Get action for edit
        /// </summary>
        /// <param name="action_id">Action Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get")]
        public async Task<ActionResult<Models.Action>> GetAction([FromQuery] int action_id)
        {
            var action = await _context.Actions.Include(a => a.Description.Translations).Include(a => a.Name.Translations).
                FirstOrDefaultAsync(a => a.Id == action_id);

            if (action == null) return BadRequest(new Result
            {
                Status = "Error",
                Message = "Action not found!"
            });

            var response = new Models.Action
            {
                id = action.Id,
                names = action.Name.Translations.ToDictionary(t => t.LanguageId ?? "", t => t.TranslationString ?? ""),
                descriptions = action.Description.Translations.ToDictionary(t => t.LanguageId ?? "", t => t.TranslationString ?? ""),
                status = action.Status,
                start_date = action.StartDate,
                end_date = action.EndDate,
                banner_big = Request.GetImageURL(BucketNames.banner, action.BannerBig),
                banner_small = Request.GetImageURL(BucketNames.banner, action.BannerSmall)

            };

            return Ok(response);
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<Models.Action>> CreateAction([FromForm] string action_json, IFormFile? big_banner, IFormFile? small_banner)
        {
            try
            {
                var action = JsonConvert.DeserializeObject<Models.Action>(action_json);
                if (action == null) return BadRequest(new Result
                {
                    Status = "Error",
                    Message = "Action_data json string is bad - check data!"
                });

                var new_action = new ActionModel
                {

                    Name = TextContentHelper.CreateFromDictionary(_context, action.names, false),
                    Description = TextContentHelper.CreateFromDictionary(_context, action.descriptions, false),
                    BannerBig = big_banner == null ? "" : await _fileService.SaveFileFromStream(BucketNames.banner.ToString(), big_banner.FileName, big_banner.OpenReadStream()),
                    BannerSmall = small_banner == null ? "" : await _fileService.SaveFileFromStream(BucketNames.banner.ToString(), small_banner.FileName, small_banner.OpenReadStream()),
                    Status = action.status,
                    StartDate = action.start_date ?? DateTime.Now,
                    EndDate = action.end_date,
                };

                _context.Actions.Add(new_action);
                _context.SaveChanges();
                action.id = new_action.Id;

                return new JsonResult(action);
            }
            catch (Exception e)
            {
                return BadRequest(new Result
                {
                    Status = "Error",
                    Message = "Action_data json string is bad - check data!",
                    Entity = "Create action error: " + e.Message
                });
            }
        }

        [HttpPut]
        [Route("update")]
        public async Task<ActionResult<Models.Action>> UpdateAction([FromForm] string action_json, IFormFile? big_banner, IFormFile? small_banner)
        {
            try
            {

                var action = JsonConvert.DeserializeObject<Models.Action>(action_json);
                if (action == null) return BadRequest(new Result
                {
                    Status = "Error",
                    Message = "Action_data json string is bad - check data!"
                });

                var exist_action = await _context.Actions.Include(a => a.Name.Translations).Include(a => a.Description.Translations).FirstOrDefaultAsync(a => a.Id == action.id);
                if (exist_action == null) return BadRequest(new Result
                {
                    Status = "Error",
                    Message = $"Action with id {action.id} not found !"
                });

                TextContentHelper.UpdateFromDictionary(_context, exist_action.Name, action.names, false);
                TextContentHelper.UpdateFromDictionary(_context, exist_action.Description, action.descriptions, false);

                if (big_banner != null)
                {
                    _fileService.DeleteFile(BucketNames.banner.ToString(), exist_action.BannerBig);
                    exist_action.BannerBig = await _fileService.SaveFileFromStream(BucketNames.banner.ToString(), big_banner.FileName, big_banner.OpenReadStream());
                }

                if (small_banner != null)
                {
                    _fileService.DeleteFile(BucketNames.banner.ToString(), exist_action.BannerSmall);
                    exist_action.BannerSmall = await _fileService.SaveFileFromStream(BucketNames.banner.ToString(), small_banner.FileName, big_banner.OpenReadStream());
                }

                exist_action.Status = action.status;
                exist_action.StartDate = action.start_date ?? exist_action.StartDate;
                exist_action.EndDate = action.end_date;


                _context.Actions.Update(exist_action);
                _context.SaveChanges();

                return new JsonResult(action);
            }
            catch (Exception e)
            {
                return BadRequest(new Result
                {
                    Status = "Error",
                    Message = "Action_data json string is bad - check data!",
                    Entity = "Update action error: " + e.Message
                });
            }
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<ActionResult<Models.Action>> DeleteAction([FromQuery] int action_id)
        {
            try
            {
                var action = await _context.Actions.Include(a => a.Articles).FirstOrDefaultAsync(a => a.Id == action_id);

                if (action == null) throw new Exception("Action with id {action_id} not found !");

                if (action.Status.Equals("active") && action.Articles.Count > 0) throw new Exception("Action is active and have articles related!");

                _context.Actions.Remove(action);
                _context.SaveChanges();

                return Ok(new Result
                {
                    Status = "Success",
                    Message = $"Action successfully deleted!"
                });
            }
            catch (Exception e)
            {
                return BadRequest(new Result
                {
                    Status = "Error",
                    Message = "Action_data json string is bad - check data!",
                    Entity = "Delete action error: " + e.Message
                });
            }
        }
    }
}
