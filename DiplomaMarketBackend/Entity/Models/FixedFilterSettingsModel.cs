namespace DiplomaMarketBackend.Entity.Models
{
    public class FixedFilterSettingsModel
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }
        public CategoryModel? Category { get; set; }

        public int PriceStep { get; set; }

        public bool IsBrandFilterEnabled { get; set; } = true;
        public bool IsPriceFilterEnabled { get; set; } = true;
        public bool IsReadyToShipFilterEnabled { get; set; } = true;
        public bool IsActionsFilterEnabled { get; set; } = true;
        public bool IsLoyalityFilterEnabled { get; set; } = true;
        public bool IsStatusFilterEnabled { get; set; } = true;

    }
}
