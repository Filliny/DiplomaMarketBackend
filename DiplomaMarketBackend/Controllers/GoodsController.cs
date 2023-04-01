using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using Microsoft.AspNetCore.Mvc;

namespace DiplomaMarketBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoodsController : Controller
    {
        ILogger<FilesController> _logger;
        ICloudStorageService _storageService;
        BaseContext _context;
        IFileService _fileService;

        public GoodsController(ILogger<FilesController> logger, ICloudStorageService cloudStorageService, BaseContext context, IFileService fileService)
        {
            _logger = logger;
            _storageService = cloudStorageService;
            _context = context;
            _fileService = fileService;
        }

        [HttpGet]
        [Route("{bucket}/{id}")]
        public async Task<IActionResult> bigImage(string bucket,string id)
        {

            string MimeType = "image/jpg";
            var img_id = id.Split('.')[0];

            var bytes = _fileService.GetFile(img_id, bucket).Result;
            return File(bytes, MimeType);
        }
    }
}
