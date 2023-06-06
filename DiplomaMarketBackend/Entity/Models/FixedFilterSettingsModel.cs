namespace DiplomaMarketBackend.Entity.Models
{
    public class FixedFilterSettingsModel
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }
        public CategoryModel? Category { get; set; }

        public int PriceStep { get; set; }

        public bool ShowBrands { get; set; } = true;
        public bool ShowPrice { get; set; } = true;
        public bool ShowShip { get; set; } = true;
        public bool ShowActions { get; set; } = true;
        public bool ShowLoyality { get; set; } = true;
        public bool ShowStatus { get; set; } = true;

    }
}
