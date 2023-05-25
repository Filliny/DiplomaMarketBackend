namespace DiplomaMarketBackend.Entity.Models
{
    public class BannerModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; }

    }
}
