namespace DiplomaMarketBackend.Entity.Models.Delivery
{
    public class CityModel
    {

        public int Id { get; set; }
        public TextContent? Name { get; set; }
        public int ? AreaId { get; set; }
        public AreaModel? Area { get; set; }
        public string Lat { get; set; } = string.Empty;
        public string Long { get; set; } = string.Empty;
        public string NpCityRef { get; set; } = string.Empty;
        public string CoatsuCode { get; set; } = string.Empty;
        public string Index1 { get; set; } = string.Empty;
        public string Index2 { get; set; } = string.Empty;
        public List<BranchModel> Branches { get; set; }

        public CityModel()
        {
            Branches = new();
        }
    }
}
