namespace DiplomaMarketBackend.Parser.Characteristics
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Datum
    {
        public int group_id { get; set; }
        public int group_order { get; set; }
        public string groupTitle { get; set; }
        public List<Option> options { get; set; }
    }

    public class Option
    {
        public int id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public int category_id { get; set; }
        public string status { get; set; }
        public int order { get; set; }
        public int in_short { get; set; }
        public string comparable { get; set; }
        public List<Value> values { get; set; }
    }

    public class Root
    {
        public List<Datum> data { get; set; }
        public bool success { get; set; }
    }

    public class Value
    {
        public string title { get; set; }
        public string href { get; set; }
    }

}
