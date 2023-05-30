namespace DiplomaMarketBackend.Models
{
    public class Action
    {
        /// <summary>
        /// Action Id
        /// </summary>
        public int Id { get; set; }

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

        public DateTime  StartDate { get; set; }

        public DateTime EndDate { get; set; }

    }
}
