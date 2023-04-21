using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace DiplomaMarketBackend.Controllers
{
    public class OrdersController : Controller
    {
        ILogger<WorkController> _logger;
        BaseContext _context;
        IFileService _fileService;

        public OrdersController(ILogger<WorkController> logger, BaseContext context, IFileService fileService)
        {
            _logger = logger;
            _context = context;
            _fileService = fileService;
        }

        [HttpPost]
        [Route("new")]
        public async Task<ActionResult<Order>> NewOrder([FromBody] Order order )
        {
            order.goods.Add(1, 3);
            order.goods.Add(5, 2);

            return Ok(order);
        }
    }
}
