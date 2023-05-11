using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using Microsoft.AspNetCore.Mvc;

namespace DiplomaMarketBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReferenceController : Controller
    {
        ILogger<WorkController> _logger;
        BaseContext _context;
        IFileService _fileService;

        public ReferenceController(ILogger<WorkController> logger, BaseContext context, IFileService fileService)
        {
            _logger = logger;
            _context = context;
            _fileService = fileService;
        }

        /// <summary>
        /// System langages reference list
        /// </summary>
        /// <returns>List of ids:languages</returns>
        [HttpGet]
        [Route("languages")]
        [ResponseCache(Duration =36000)]
        public async Task<IActionResult> Languages()
        {
            var languages = _context.Languages.ToList();

            return new JsonResult(languages);
        }

        /// <summary>
        /// List of comparability values
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("comparability")]
        [ResponseCache(Duration = 36000)]
        public IActionResult Comparability() {

            var compares = new string[] { "main", "disable", "advanced", "bottom" };

            return new JsonResult(new { data = compares });

        }

    }
}
