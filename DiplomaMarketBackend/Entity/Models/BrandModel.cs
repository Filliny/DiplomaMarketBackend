namespace DiplomaMarketBackend.Entity.Models
{
    public class BrandModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? LogoURL { get; set; }

        public int rztk_brand_id { get; set; }
    }
}
