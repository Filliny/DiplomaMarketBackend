using Microsoft.EntityFrameworkCore;

namespace DiplomaMarketBackend.Models
{
    public class Article
    {
        public class PreviewImage
        {
            public int id { get; set; }
            public string url { get; set; }
        }

        public class TypeValue
        {
            public int value_id { get; set; }
            public string value_name { get; set; } = string.Empty;
            public int charcteristic_id { get; set; }
            public string charcteristic_name { get; set; } = string.Empty;
        }

        public class Warning
        {
            public int id { get; set; }
            public Dictionary<string, string> messages { get; set; }
        }

        /// <summary>
        /// Article id
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Article title translations in dictionary "locale_id":"translation"
        /// </summary>
        public Dictionary<string,string> titles { get; set; } = new Dictionary<string,string>();

        /// <summary>
        /// Article description translations in dictionary "locale_id":"translation"
        /// </summary>
        public Dictionary<string, string> descriptions { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Article docket translations in dictionary "locale_id":"translation"
        /// </summary>
        public Dictionary <string, string> dockets { get; set;} = new Dictionary<string, string>();

        /// <summary>
        /// Current price
        /// </summary>
        public decimal price { get; set; }

        /// <summary>
        /// Old price
        /// </summary>
        public decimal old_price { get; set; }

        /// <summary>
        /// Category Id
        /// </summary>
        public int category_id { get; set; }

        /// <summary>
        /// Brand Id
        /// </summary>
        public int brand_id { get; set; }

        /// <summary>
        /// Status (active/inactive)
        /// </summary>
        public string status { get; set;} = "active";

        /// <summary>
        /// Selling status (available/unavailable)
        /// </summary>
        public string sell_status { get; set; } = "available";

        /// <summary>
        /// Article warnings List - each with translations in dictionary "locale_id":"translation"
        /// </summary>
        public List<Warning> warnings { get; set; } = new List<Warning>();

        /// <summary>
        /// Url strings of videos
        /// </summary>
        public List<string> video_urls { get; set; } = new List<string>();

        /// <summary>
        /// images entities with image id and preview url for admin
        /// </summary>
        public List<PreviewImage> images { get; set; } = new List<PreviewImage>();

        /// <summary>
        /// Article attributes (characterisitc values) list with characterisitc name and id
        /// </summary>
        public List<TypeValue> values { get; set; } = new List<TypeValue>();

        /// <summary>
        /// List of actions ids
        /// </summary>
        public List<int> actions_ids { get; set; } = new List<int>();

        /// <summary>
        /// Seller id
        /// </summary>
        public int seller_id { get; set; } = 1;

        /// <summary>
        /// Article bonuses count
        /// </summary>
        public int points { get; set; }

    }
}
