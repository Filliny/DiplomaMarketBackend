namespace DiplomaMarketBackend.Models
{
    public class Service
    {
        public int Id { get; set; }

        /// <summary>
        /// Service name
        /// </summary>
        public string name { get; set; } = string.Empty;   
        
        /// <summary>
        /// Service Address
        /// </summary>
        public string address { get; set; } = string.Empty;

        /// <summary>
        /// Service phone
        /// </summary>
        public string phone { get; set; } = string.Empty;

        /// <summary>
        /// Service work hours
        /// </summary>
        public string work_hours { get; set; } = string.Empty;

        /// <summary>
        /// City name for new city creation 
        /// </summary>
        public string? city_name { get; set; } = string.Empty;

        /// <summary>
        /// Id of existing city found in Reference section of API
        /// </summary>
        public int  city_id { get; set; }

        /// <summary>
        /// Id of existing area found in Reference section of API
        /// </summary>
        public int? area_id { get; set; }

        /// <summary>
        /// Id of brand related to service company
        /// </summary>
        public int brand_id { get; set; }

        /// <summary>
        /// List of categories related to service
        /// </summary>
        public int category_id { get; set; }
    }
}
