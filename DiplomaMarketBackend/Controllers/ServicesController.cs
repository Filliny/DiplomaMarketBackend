using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Models;
using DiplomaMarketBackend.Entity.Models.Delivery;
using DiplomaMarketBackend.Helpers;
using DiplomaMarketBackend.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DiplomaMarketBackend.Controllers
{

	[ApiController]
	[Route("api/[controller]")]
	public class ServicesController : Controller
	{
		ILogger<ServicesController> _logger;
		BaseContext _context;


		public ServicesController(ILogger<ServicesController> logger, BaseContext context)
		{
			_logger = logger;
			_context = context;
		}

		/// <summary>
		/// Get all brand services
		/// </summary>
		/// <param name="brand_id">Brand id from brands</param>
		/// <returns>List of service centers for brand</returns>
		[HttpGet]
		[Route("get-by-brand")]
		public async Task<IActionResult> GetBrandServices([FromQuery] int brand_id)
		{

			var services = await _context.Services.Where(s=>s.BrandId ==  brand_id).ToListAsync();

			var response = new List<dynamic>();

			foreach (var service in services)
			{
				var company = service.Adapt<Service>();

				response.Add(company);
			}

			return Json(response);
		}

		/// <summary>
		/// Get service data by id
		/// </summary>
		/// <param name="service_id"></param>
		/// <returns></returns>
        [HttpGet]
        [Route("get-by-id")]
        public async Task<ActionResult<Service>> GetService([FromQuery] int service_id)
        {
			
            var service = await _context.Services.FindAsync(service_id);
			if (service == null) return BadRequest(new Result
			{
				Status = "Error",
				Message = "Service not found!"
			});

            var company = service.Adapt<Service>();
           

            return Json(company);
        }

		/// <summary>
		/// Create new service company
		/// </summary>
		/// <param name="service">Service data from json</param>
		/// <returns>Ok if success</returns>
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<Service>> CreateService([FromBody] Service service)
		{
			var entity = service.Adapt<ServiceModel>();

			if(service.city_id == 0 && !service.city_name.IsNullOrEmpty() && service.area_id != null)
			{
				var new_city = new CityModel
				{
					Name = TextContentHelper.CreateFull(_context, service.city_name, service.city_name),
					AreaId = service.area_id
				};

				entity.City = new_city;
			}

			_context.Services.Add(entity);
            await _context.SaveChangesAsync();

            service.Id = entity.Id;

			return Ok(new Result
			{
				Status = "Success",
				Message = "Sevice successfully created!",
				Entity = service
			});
		}


        /// <summary>
        /// Update service company
        /// </summary>
        /// <param name="service">Service data from json</param>
        /// <returns>Ok if success</returns>
        [HttpPut]
        [Route("update")]
        public async Task<ActionResult<Service>> UpdateService([FromBody] Service service)
        {
			var exist = await _context.Services.FindAsync(service.Id);

			if (exist == null) return NotFound(new Result
			{
				Status = "Error",
				Message = "Service not found",
				Entity = service
			});

            service.Adapt(exist);

            if (service.city_id == 0 && !service.city_name.IsNullOrEmpty())
            {
                var new_city = new CityModel
                {
                    Name = TextContentHelper.CreateFull(_context, service.city_name, service.city_name),
                    AreaId = service.area_id
                };

                exist.City = new_city;
            }

            _context.Services.Update(exist);
            await _context.SaveChangesAsync();

            return Ok(new Result
            {
                Status = "Success",
                Message = "Sevice successfully updated!",
                Entity = service
            });
        }

        /// <summary>
        /// Remove service company
        /// </summary>
        /// <param name="service_id">Service id </param>
        /// <returns>Ok if success</returns>
        [HttpDelete]
        [Route("delete")]
        public async Task<ActionResult<Service>> UpdateService([FromQuery] int service_id)
        {
            var exist = await _context.Services.FindAsync(service_id);

            if (exist == null) return NotFound(new Result
            {
                Status = "Error",
                Message = "Service not found",
                Entity = service_id
            });

            _context.Services.Remove(exist);
            await _context.SaveChangesAsync();

            return Ok(new Result
            {
                Status = "Success",
                Message = "Sevice successfully deleted!",
                Entity = exist
            });
        }
    }
}
