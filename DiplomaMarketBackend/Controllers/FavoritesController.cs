using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomaMarketBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoritesController : Controller
    {
        ILogger<FavoritesController> _logger;
        BaseContext _context;
        UserManager<UserModel> _userManager;

        public FavoritesController(ILogger<FavoritesController> logger, BaseContext context, UserManager<UserModel> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Get users favorites
        /// </summary>
        /// <returns>Ok if succes</returns>
        [Authorize]
        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> Get()
        {

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null) return Unauthorized();

            var full_user = _context.Users.Include(u => u.Favorites).FirstOrDefault(u => u.Id == user.Id);
            if (full_user == null) return Unauthorized();

            var ids = full_user.Favorites.Select(f => f.Id).ToList();

            return Json(ids);

        }


        /// <summary>
        /// Add to favorites
        /// </summary>
        /// <returns>Ok if succes</returns>
        [Authorize]
        [HttpGet]
        [Route("add")]
        public async Task<IActionResult> Add([FromQuery] int article_id)
        {
            var article = await _context.Articles.FindAsync(article_id);

            if (article == null) return NotFound("Article not found!");

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null) return Unauthorized();


            user.Favorites.Add(article);
            _context.SaveChanges();

            return Ok();

        }

        /// <summary>
        /// Remove from favorites
        /// </summary>
        /// <returns>Ok if succes</returns>
        [Authorize]
        [HttpGet]
        [Route("remove")]
        public async Task<IActionResult> Remove([FromQuery] int article_id)
        {
            var article = await _context.Articles.FindAsync(article_id);

            if (article == null) return NotFound("Article not found!");

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null) return Unauthorized();

            var full_user = _context.Users.Include(u => u.Favorites).FirstOrDefault(u => u.Id == user.Id);
            if (full_user == null) return Unauthorized();

            full_user.Favorites.Remove(article);
            _context.SaveChanges();

            return Ok();

        }


    }
}
