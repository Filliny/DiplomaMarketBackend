using DiplomaMarketBackend.Entity.Models;

namespace DiplomaMarketBackend.Models
{
    public class FixedFilter
    {
        public class CharacteristicType
        {
            public int id { get; set; } 
            public Dictionary <string,string> characteristic_names { get; set; } = new Dictionary<string,string>();
            public List<ValueType> values { get; set; } = new List<ValueType>();
        }

        public class ValueType
        {
            public int id { get; set; }
            public Dictionary<string, string> value_names { get; set; } = new Dictionary<string, string>();
        }

        public int category_id { get; set; }
        public string category_name { get; set; } = string.Empty;
        public bool is_brand_filer_enabled { get; set; }
        public bool is_price_filer_enabled { get; set; }
        public bool is_ready_to_ship_filter_enabled { get; set; }
        public bool is_actions_filter_enabled { get; set; }
        public bool is_loyality_filter_enabled { get; set; } 
        public bool is_status_filter_enabled { get; set; }

        public List<CharacteristicType>? characteristics { get; set; } = new List<CharacteristicType>();

    }
}
