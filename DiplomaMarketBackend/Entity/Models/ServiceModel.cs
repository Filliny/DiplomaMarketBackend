using DiplomaMarketBackend.Entity.Models.Delivery;

namespace DiplomaMarketBackend.Entity.Models
{
    public class ServiceModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int CityId { get; set; }

        public CityModel? City { get; set; }

        public string Address { get; set; } = string.Empty;

        public int BrandId { get; set; } 

        public BrandModel? Brand { get; set;}

        public int CategoryId { get; set;}

        public CategoryModel? Category { get; set;}

        public string Phone { get; set; } = string.Empty;

        public string WorkHours { get; set; } = string.Empty;

    }
}
