namespace DiplomaMarketBackend.Parser.Mist.Branches
{
    public class City
    {
        public string ua { get; set; }
        public string ru { get; set; }
        public string en { get; set; }
    }

    public class Result
    {
        public string br_id { get; set; }
        public City city { get; set; }
        public string city_id { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string num { get; set; }
        public int num_showcase { get; set; }
        public int parcel_max_kg { get; set; }
        public string type_id { get; set; }
        public TypePublic type_public { get; set; }
    }

    public class Root
    {
        public int status { get; set; }
        public object msg { get; set; }
        public List<Result> result { get; set; }
    }

    public class TypePublic
    {
        public string ua { get; set; }
        public string ru { get; set; }
        public string en { get; set; }
    }


}
