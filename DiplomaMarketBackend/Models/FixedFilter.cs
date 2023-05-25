using DiplomaMarketBackend.Entity.Models;

namespace DiplomaMarketBackend.Models
{
    public class FixedFilter
    {
        public int id {  get; set; }

        public int category_id { get; set; }
        public string category_name { get; set; } = string.Empty;
        public bool is_brand_filer_enabled { get; set; }
        public bool is_price_filer_enabled { get; set; }
        public bool is_ready_to_ship_filter_enabled { get; set; }
        public bool is_actions_filter_enabled { get; set; }
        public bool is_loyality_filter_enabled { get; set; } 
        public bool is_status_filter_enabled { get; set; }

        public List<string> locales_available { get; set; } = new List<string>();

        public Dictionary<string, string>? brand_filter_name { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, string>? price_filter_name { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, string>? ready_to_ship_filter_name { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string>? ready_to_ship_value_name { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, string>? actions_filter_name { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string>? action_value_name { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, string>? loyality_filter_name { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string>? with_bonuses_value_name { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, string>? article_status_filter_name { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string>? in_stock_value_name { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string>? out_of_stock_value_name { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string>? item_ended_value_name { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string>? ending_soon_value_name { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string>? item_awaited_value_name { get; set; } = new Dictionary<string, string>();
    }
}
