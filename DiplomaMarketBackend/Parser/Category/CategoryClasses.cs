namespace DiplomaMarketBackend.Parser.Category
{
    public class Alternate
    {
        public string lang { get; set; }
        public string hreflang { get; set; }
        public string domain { get; set; }
        public string subdomain { get; set; }
    }

    public class Data
    {
        public List<int> ids { get; set; }
        public int ids_count { get; set; }
        public int total_pages { get; set; }
        public int show_next { get; set; }
        public int goods_with_filter { get; set; }
        public int goods_in_category { get; set; }
        public int goods_limit { get; set; }
        public List<int> active_pages { get; set; }
        public int shown_page { get; set; }
        public List<Alternate> alternate { get; set; }
    }

    public class Root
    {
        public Data data { get; set; }
    }

}
