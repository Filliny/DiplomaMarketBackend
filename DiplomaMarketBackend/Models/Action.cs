namespace DiplomaMarketBackend.Models
{
    public class Action
    {
        /// <summary>
        /// Action Id
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Names localization dictionary
        /// </summary>
        public Dictionary<string,string> names { get; set; } = new Dictionary<string,string>();

        /// <summary>
        /// Descriptions localizatuion dictionary
        /// </summary>
        public Dictionary<string, string> descriptions { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Action status = "pending","active","ended"
        /// </summary>
        public string status { get; set; } = string.Empty;

        /// <summary>
        /// Create date of action or start date for date-planned action
        /// </summary>
        public DateTime?  start_date { get; set; }

        /// <summary>
        /// Action end day
        /// </summary>
        public DateTime end_date { get; set; }

        /// <summary>
        /// Image banner big preview link
        /// </summary>
        public string? banner_big { get; set; }

        /// <summary>
        /// Image banner small preview link 
        /// </summary>
        public string? banner_small { get; set;}

    }
}
