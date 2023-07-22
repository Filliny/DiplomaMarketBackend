using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Models;
using DiplomaMarketBackend.Helpers;
using DiplomaMarketBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace DiplomaMarketBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StorageController:ControllerBase
{
    ILogger<StorageController> _logger;
    BaseContext _context;
    IFileService _fileService;

    public StorageController(ILogger<StorageController> logger, BaseContext context, IFileService fileService)
    {
        _logger = logger;
        _context = context;
        _fileService = fileService;
    }

    /// <summary>
    /// List of articles for ordering to refill
    /// Товари на поповнення
    /// </summary>
    /// <param name="lang">Language</param>
    /// <param name="search">Search string</param>
    /// <param name="article_id">Article id</param>
    /// <param name="category_id">Category id</param>
    /// <param name="page">Page to display</param>
    /// <param name="limit">Articles to display on page</param>
    /// <param name="only_ending">Show only needed refilling articles (sell statuses !available || !awaiting)</param>
    /// <returns></returns>
    [HttpGet]
    [Route("refill_list")]
    public async Task<IActionResult> GetRefillList([FromQuery, BindRequired] string lang, string? search, int? article_id, int? category_id, int page = 1, int limit = 10, bool only_ending=false )
    {
        lang = lang.NormalizeLang();

        var articles = new List<dynamic>();

        var goods = _context.Articles.AsSplitQuery().
            Include(a => a.Title.Translations).
            Include(a => a.Category).
            Include(a => a.Images).ThenInclude(p => p.small).
            Include(a => a.Category.Name.Translations).AsNoTracking();


        if (category_id is not null)
        {
            goods = goods.Where(a => a.CategoryId == category_id);
        }

        if (article_id is not null)
        {
            goods = goods.Where(a => a.Id == article_id);
        }

        if (search is not null)
        {
            goods = goods.Where(c => c.Title.Translations.Any(t => t.TranslationString.ToLower().Contains(search.ToLower()) && t.LanguageId == lang));
        }

        if (only_ending)
        {
            goods = goods.Where(a => !a.SellStatus.Equals("available")).Where(a => !a.SellStatus.Equals("awaiting"));
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
                status = article.Status,
                sell_status = article.SellStatus,
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

        return Ok(result);

    }

    /// <summary>
    /// Change order status by id and status id 
    /// </summary>
    /// <param name="order_id">Order id</param>
    /// <param name="status">Status id from /api/References/order-status</param>
    /// <returns>Ok if success</returns>
    [HttpPut]
    [Route("change-status")]
    //[Authorize(Roles = "OrdersWrite")]
    public async Task<IActionResult> ChangeOrderStatus([FromQuery] int order_id, int status)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == order_id);

        if (order == null)
            return NotFound(new Result
            {
                Status = "Error",
                Message = "Oder not found!"

            });

        order.Status = (OrderStatus)status;
        await _context.SaveChangesAsync();
        
        return Ok(
            new Result
            {
                Status = "Success",
                Message = "Order updated successfully!"

            });
    }
    
    /// <summary>
    /// Close orders given from list of Ids
    /// </summary>
    /// <param name="orders_ids">List of ids</param>
    /// <returns>Ok if Success</returns>
    [HttpPut]
    [Route("close-orders")]
    //[Authorize(Roles = "OrdersWrite")]
    public async Task<IActionResult> CloseOrders([FromBody] int[] orders_ids)
    {
        foreach (var id in orders_ids)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
            
            if (order == null)
                return NotFound(new Result
                {
                    Status = "Error",
                    Message = $"Oder id:{id} not found!"

                });

            order.Status = OrderStatus.Closed;
        }
        
        await _context.SaveChangesAsync();
        return Ok(
            new Result
            {
                Status = "Success",
                Message = "Orders closed successfully!"

            });
      
    }
    
    /// <summary>
    /// List of closed orders
    /// </summary>
    /// <param name="start_date">Start date</param>
    /// <param name="end_date">End date</param>
    /// <param name="lang">Language</param>
    /// <param name="page">Page to show</param>
    /// <param name="limit">Orders on page</param>
    /// <returns></returns>
    [HttpGet]
    [Route(("closed-list"))]
    public async Task<IActionResult> ClosedOrders([FromQuery] DateTime? start_date, DateTime? end_date, string lang, int page=1, int limit=10)
        {
            var ordersq = _context.Orders.
                Include(o=>o.User).
                OrderBy(o=>o.CreatedAt).
                Where(o => o.Status == OrderStatus.Closed);

            if (start_date > end_date)
            {
                (end_date, start_date) = (start_date, end_date);
            }

            if (start_date is not null)
            {
                ordersq = ordersq.Where(o => o.CreatedAt >= start_date);
            }

            if (end_date is not null)
            {
                ordersq = ordersq.Where(o => o.CreatedAt <= end_date);
            }


            var orders = await ordersq.ToListAsync();
            

            int totalOrders = orders.Count;
            int totalPages = (int)Math.Ceiling((decimal)totalOrders / (decimal)limit);
            
            if (page > totalPages) page = totalPages;
            int skip = (page - 1) * limit;
            
            orders = orders.Skip(skip).Take(limit).ToList();

            var outlist = new List<dynamic>();
            foreach (var order in orders)
            {
                var userOrders = await _context.Orders.OrderBy(o => o.CreatedAt).
                    Where(o => o.UserId == order.UserId)
                    .ToListAsync();
                
                outlist.Add(new
                {
                    id = order.Id,
                    payment_status = "payed",
                    orders = userOrders.Count.ToString()+" від "+userOrders.First().CreatedAt.ToString("dd MM yyyy"),
                    buyer_name = order.User?.LastName??""+" "+ order.User?.FirstName??"",
                    phone = order.User?.PhoneNumber,
                    order_date = order.CreatedAt.ToString("dd MM yyyy"),
                    status = order.Status
                    
                });
            }
            
            var result = new
            {
                data = new
                {
                    orders = outlist,
                    total_goods = totalOrders,
                    total_pages = totalPages,
                    displayed_page = page
                }
            };

            return Ok(result);
        }
}