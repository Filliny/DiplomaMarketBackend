namespace DiplomaMarketBackend.Entity.Models.Delivery
{
    public class AreaModel
    {
        public int Id { get; set; }

        public int? NameId { get; set; }
        public TextContent? Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string DescriptionRu { get; set; } = string.Empty;
        public string NpRef { get; set; } = string.Empty;
        public string NpAreaCenterRef { get; set; } = string.Empty;
        public string MistRegionId { get; set; } = string.Empty;
    }
}
