namespace DiplomaMarketBackend.Entity.Models.Delivery
{
    public class BranchModel
    {
        public int Id { get; set; }
        public string? DeliveryBranchId { get; set; }
        public int? DeliveryId { get; set; }
        public DeliveryModel? Delivery { get; set; }
        public string LocalBranchNumber { get; set; } = string.Empty;
        public int? BranchCityId { get; set; }
        public CityModel? BranchCity { get; set; }

        public TextContent? Description { get; set; }

        public TextContent? Address { get; set; }

        public string Long { get; set; } =string.Empty;
        public string Lat { get; set; } = string.Empty;

        public string WorkHours { get; set; } = string.Empty;
        
        public DateTime? Updated{ get; set;}




    }
}
