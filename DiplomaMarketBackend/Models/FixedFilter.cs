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

        public int id { get; set; }
        public int category_id { get; set; }
        public string? category_name { get; set; } 
        public bool show_brands { get; set; }
        public bool show_price { get; set; }
        public bool show_ship { get; set; }
        public bool show_actions { get; set; }
        public bool show_loyality { get; set; } 
        public bool show_status { get; set; }

        public List<CharacteristicType>? characteristics { get; set; } = new List<CharacteristicType>();

    }
}
