namespace DiplomaMarketBackend.Parser.NewPost.Cities
{

    public class Datum
    {
        public string Ref { get; set; }
        public string SettlementType { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Description { get; set; }
        public string DescriptionRu { get; set; }
        public string DescriptionTranslit { get; set; }
        public string SettlementTypeDescription { get; set; }
        public string SettlementTypeDescriptionRu { get; set; }
        public string SettlementTypeDescriptionTranslit { get; set; }
        public string Region { get; set; }
        public string RegionsDescription { get; set; }
        public string RegionsDescriptionRu { get; set; }
        public string RegionsDescriptionTranslit { get; set; }
        public string Area { get; set; }
        public string AreaDescription { get; set; }
        public string AreaDescriptionRu { get; set; }
        public string AreaDescriptionTranslit { get; set; }
        public string Index1 { get; set; }
        public string Index2 { get; set; }
        public string IndexCOATSU1 { get; set; }
        public string Delivery1 { get; set; }
        public string Delivery2 { get; set; }
        public string Delivery3 { get; set; }
        public string Delivery4 { get; set; }
        public string Delivery5 { get; set; }
        public string Delivery6 { get; set; }
        public string Delivery7 { get; set; }
        public int SpecialCashCheck { get; set; }
        public string Warehouse { get; set; }
    }

    public class Info
    {
        public int totalCount { get; set; }
    }

    public class Root
    {
        public bool success { get; set; }
        public List<Datum> data { get; set; }
        public List<object> errors { get; set; }
        public List<object> warnings { get; set; }
        public Info info { get; set; }
        public List<object> messageCodes { get; set; }
        public List<object> errorCodes { get; set; }
        public List<object> warningCodes { get; set; }
        public List<object> infoCodes { get; set; }
    }


}
