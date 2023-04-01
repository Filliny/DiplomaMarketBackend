namespace DiplomaMarketBackend.Parser.Categories
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Category:ICategory
    {
        public int? id { get; set; }
        public int? order { get; set; }
        public string title { get; set; }
        public int? parent_id { get; set; }
        public string manual_url { get; set; }
        public int? border { get; set; }
        public List<Logo> logos { get; set; }
        public object icon_src { get; set; }
        public int? category_id { get; set; }
        public int? top_category_id { get; set; }
        public int? target_blank { get; set; }
        public string banner_mobile_src { get; set; }
        public int? on_subdomain { get; set; }
        public bool? is_portal { get; set; }
        public string banner_href { get; set; }
        public string banner_src { get; set; }
        public Child children { get; set; }
        public List<PopularCategory> popular_categories { get; set; }
        public bool? outer_link { get; set; }
    }

    public class Child:ICategory
    {
        public int? id { get; set; }
        public string title { get; set; }
        public string manual_url { get; set; }
        public int? category_id { get; set; }
        public int? top_category_id { get; set; }
        public int? on_subdomain { get; set; }
        public bool? is_portal { get; set; }
        public bool? outer_link { get; set; }
        public List<One> one { get; set; }
        public List<Two> two { get; set; }
        public List<Three> three { get; set; }
    }

    public class Data
    {
        public Category Category { get; set; }
    }

    public class Logo
    {
        public int? id { get; set; }
        public string logo { get; set; }
        public string manual_url { get; set; }
        public string name { get; set; }
        public string title { get; set; }
    }

    public class One:ICategory
    {
        public int? id { get; set; }
        public string title { get; set; }
        public int? parent_id { get; set; }
        public string column_number { get; set; }
        public string manual_url { get; set; }
        public int? category_id { get; set; }
        public int? top_category_id { get; set; }
        public int? on_subdomain { get; set; }
        public bool? is_portal { get; set; }
        public List<Child> children { get; set; }
        public bool? outer_link { get; set; }
    }

    public class PopularCategory
    {
        public string link_url { get; set; }
        public string title { get; set; }
        public int? id { get; set; }
        public int? parent_id { get; set; }
        public int? order { get; set; }
        public int? top_parent_id { get; set; }
    }

    public class Root
    {
        public Data data { get; set; }
    }

    public class Three:ICategory
    {
        public int? id { get; set; }
        public string title { get; set; }
        public int? parent_id { get; set; }
        public string column_number { get; set; }
        public string manual_url { get; set; }
        public int? category_id { get; set; }
        public int? top_category_id { get; set; }
        public int? on_subdomain { get; set; }
        public bool? is_portal { get; set; }
        public List<Child> children { get; set; }
        public bool? outer_link { get; set; }
    }

    public class Two:ICategory
    {
        public int? id { get; set; }
        public string title { get; set; }
        public int? parent_id { get; set; }
        public string column_number { get; set; }
        public string manual_url { get; set; }
        public int? category_id { get; set; }
        public int? top_category_id { get; set; }
        public int? on_subdomain { get; set; }
        public bool? is_portal { get; set; }
        public List<Child> children { get; set; }
        public bool? outer_link { get; set; }
    }


    public interface ICategory
    {

        public string title { get; set; }
        public int? category_id { get; set; }

    }

}
