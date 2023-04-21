namespace DiplomaMarketBackend.Parser.Mist.City
{
    public class Data
    {
        public string n_ua { get; set; }
        public string n_ru { get; set; }
        public string t_ua { get; set; }
        public string city_id { get; set; }
        public string kt { get; set; }
        public string reg { get; set; }
        public string reg_id { get; set; }
        public string dis { get; set; }
        public string d_id { get; set; }
        public bool is_delivery_in_city { get; set; }
    }

    public class Result
    {
        public Data data { get; set; }
    }

    public class Root
    {
        public int status { get; set; }
        public object msg { get; set; }
        public List<Result> result { get; set; }
    }
}
