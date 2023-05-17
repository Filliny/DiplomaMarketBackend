namespace DiplomaMarketBackend.Models
{
    public class Service
    {
        public int Id { get; set; }

        /// <summary>
        /// Service name
        /// </summary>
        public string Name { get; set; } = string.Empty;   
        
        /// <summary>
        /// Service Address
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Service phone
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// Service work hours
        /// </summary>
        public string WorkHours { get; set; } = string.Empty;

        /// <summary>
        /// City name for new city creation 
        /// </summary>
        public string? City { get; set; } = string.Empty;

        /// <summary>
        /// Id of existing city found in Reference section of API
        /// </summary>
        public int  CityId { get; set; }

        /// <summary>
        /// Id of existing area found in Reference section of API
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// Id of brand related to service company
        /// </summary>
        public int BrandId { get; set; }

        /// <summary>
        /// List of categories related to service
        /// </summary>
        public int CategoryId { get; set; }
    }
}
