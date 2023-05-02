namespace DiplomaMarketBackend.Models
{
    public class Characteristic
    {
        public class Value
        {
            /// <summary>
            /// Value id if existing
            /// </summary>
            public int id { get; set; }

            /// <summary>
            /// Translations dictionary "LanguageId":"TranslationString"
            /// </summary>
            public Dictionary<string,string> Translations { get; set; } = new Dictionary<string,string>();

        }

        public class Group
        {
            /// <summary>
            /// Group Id if exist
            /// </summary>
            public int id { get; set; }

            /// <summary>
            /// Order of group appearing in characteristic list
            /// </summary>
            public int show_order { get; set; }

            /// <summary>
            /// Translations dictionary "LanguageId":"TranslationString"
            /// </summary>
            public Dictionary<string, string> Translations { get; set; } = new Dictionary<string, string>();

        }

        /// <summary>
        /// characteristic Id if exist
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Names translations dictionary "LanguageId":"TranslationString"
        /// </summary>
        public Dictionary<string, string> Names { get; set; } = new Dictionary<string, string>();
        
        /// <summary>
        /// List of values
        /// </summary>
        public List<Value> Values { get; set; } = new List<Value>();

        /// <summary>
        /// Characterisitc group
        /// </summary>
        public Group? group { get; set; }

        /// <summary>
        /// Does characteristic active?
        /// </summary>
        public bool is_active { get; set;}

        /// <summary>
        /// Category id of characteristic
        /// </summary>
        public int? category_id { get; set; }

        /// <summary>
        /// Chsrscteristic showing order in group
        /// </summary>
        public int? show_order {get; set;}

        /// <summary>
        /// Comparability of characteristic in string ("main","disable","advanced","bottom")
        /// </summary>
        public string? comparable { get; set;}


    }
}
