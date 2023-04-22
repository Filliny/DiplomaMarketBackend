namespace DiplomaMarketBackend.Parser.Delengine.Cities
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Datum
    {
        public string uuid { get; set; }
        public string name_uk { get; set; }
        public string name_ru { get; set; }
        public string name_en { get; set; }
        public string katottg { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public District district { get; set; }
        public Region region { get; set; }
        public SettlementType settlement_type { get; set; }
        public int np_courier { get; set; }
    }

    public class District
    {
        public string uuid { get; set; }
        public string name_uk { get; set; }
    }

    public class Paginate
    {
        public int total { get; set; }
        public int per_page { get; set; }
        public int current_page { get; set; }
    }

    public class Region
    {
        public string uuid { get; set; }
        public string name_uk { get; set; }
    }

    public class Root
    {
        public string status { get; set; }
        public List<Datum> data { get; set; }
        public Paginate paginate { get; set; }
    }

    public class SettlementType
    {
        public string uuid { get; set; }
        public string name_uk { get; set; }
    }


}
