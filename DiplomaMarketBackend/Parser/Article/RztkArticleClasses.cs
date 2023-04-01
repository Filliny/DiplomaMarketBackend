using Newtonsoft.Json;

namespace DiplomaMarketBackend.Parser.Article
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class BaseAction
    {
        public string url { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
    }

    public class Big
    {
        public string url { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
    }

    public class BigTile
    {
        public string url { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
    }

    public class Brand
    {
        [JsonProperty("@type")]
        public string type { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Breadcrumb
    {
        public int id { get; set; }
        public string title { get; set; }
        public string href { get; set; }
    }

    public class BreadcrumbList
    {
        [JsonProperty("@context")]
        public string context { get; set; }

        [JsonProperty("@type")]
        public string type { get; set; }
        public List<ItemListElement> ItemListElement { get; set; }
    }

    public class CartQuantity
    {
        public int? min { get; set; }
        public int? max { get; set; }
    }

    public class Category
    {
        public bool isSuperMarket { get; set; }
    }

    public class Data
    {
        public int id { get; set; }
        public string title { get; set; }
        public string price { get; set; }
        public string old_price { get; set; }
        public string price_pcs { get; set; }
        public string min_month_price { get; set; }
        public string href { get; set; }
        public string name { get; set; }
        public int comments_amount { get; set; }
        public string sell_status { get; set; }
        public string status { get; set; }
        public string status_inherited { get; set; }
        public string comments_mark { get; set; }
        public int category_id { get; set; }
        public int seller_id { get; set; }
        public int merchant_id { get; set; }
        public int group_id { get; set; }
        public int producer_id { get; set; }
        public string docket { get; set; }
        public string state { get; set; }
        public object title_short { get; set; }
        public string promo_title_part { get; set; }
        public string mpath { get; set; }
        public bool is_set { get; set; }
        public string brand { get; set; }
        public int brand_id { get; set; }
        public string brand_name { get; set; }
        public string brand_logo { get; set; }
        public List<Breadcrumb> breadcrumbs { get; set; }
        public LastCategory last_category { get; set; }
        public List<Image> images { get; set; }
        public Tag tag { get; set; }
        public int sla_id { get; set; }
        public int is_markdown { get; set; }
        public bool use_group_links { get; set; }
        public List<object> custom_buttons { get; set; }
        public string article { get; set; }
        public CartQuantity cart_quantity { get; set; }
        public bool has_alternate { get; set; }
        public string canonical { get; set; }
        public bool isCanonical { get; set; }
        public int discount { get; set; }
        public string mark_percent { get; set; }
        public object adult_alert { get; set; }
        public List<string> warning { get; set; }
        public List<Video> videos { get; set; }
        public List<object> video_presentation { get; set; }
        public Description description { get; set; }
        public bool isPreorder { get; set; }
        public Seo seo { get; set; }
        public string short_title { get; set; }
        public object size_chart_link { get; set; }
        public bool size_chart_in_modal { get; set; }
        public object size_chart_goods_option { get; set; }
        public List<object> documents { get; set; }
        public object order_counter { get; set; }
        public Category category { get; set; }
        public bool forbid_comment { get; set; }
        public bool forbid_question { get; set; }
        public int code { get; set; }
    }

    public class Description
    {
        public string text { get; set; }
        public object html { get; set; }
        public bool isRichNoLimit { get; set; }
    }

    public class Image
    {
        public object id { get; set; }
        public Original original { get; set; }
        public BaseAction base_action { get; set; }
        public Preview preview { get; set; }
        public Small small { get; set; }
        public Medium medium { get; set; }
        public Large large { get; set; }
        public BigTile big_tile { get; set; }
        public Big big { get; set; }
        public MobileMedium mobile_medium { get; set; }
        public MobileLarge mobile_large { get; set; }
    }

    public class Item
    {
        [JsonProperty("@id")]
        public string id { get; set; }
        public string name { get; set; }
    }

    public class ItemListElement
    {
        [JsonProperty("@type")]
        public string type { get; set; }
        public int position { get; set; }
        public Item item { get; set; }
    }

    public class Large
    {
        public string url { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
    }

    public class LastCategory
    {
        public int id { get; set; }
        public string title { get; set; }
        public string href { get; set; }
        public string rz_mpath { get; set; }
    }

    public class Medium
    {
        public string url { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
    }

    public class MobileLarge
    {
        public string url { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
    }

    public class MobileMedium
    {
        public string url { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
    }

    public class Offers
    {
        [JsonProperty("@type")]
        public string type { get; set; }
        public string availability { get; set; }
        public string url { get; set; }
        public string price { get; set; }
        public string priceCurrency { get; set; }
        public DateTime priceValidUntil { get; set; }
    }

    public class Original
    {
        public string url { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
    }

    public class Preview
    {
        public string url { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
    }

    public class Product
    {
        [JsonProperty("@context")]
        public string context { get; set; }

        [JsonProperty("@type")]
        public string type { get; set; }
        public string sku { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public List<string> image { get; set; }
        public string description { get; set; }
        public string itemCondition { get; set; }
        public Offers offers { get; set; }
        public Brand brand { get; set; }
        public string gtin13 { get; set; }
    }

    public class Root
    {
        public Data data { get; set; }
        public bool success { get; set; }
    }

    public class Seo
    {
        public BreadcrumbList BreadcrumbList { get; set; }
        public Product Product { get; set; }
    }

    public class Small
    {
        public string url { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
    }

    public class Tag
    {
        public string name { get; set; }
        public string title { get; set; }
        public int priority { get; set; }
    }

    public class Video
    {
        public string name { get; set; }
        public int id { get; set; }
        public string type { get; set; }
        public string ext_video_id { get; set; }
        public string tag { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string created_at { get; set; }
        public string preview_url { get; set; }
        public int order { get; set; }
    }

}
