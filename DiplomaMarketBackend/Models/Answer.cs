namespace DiplomaMarketBackend.Models
{
    public class Answer
    {
        /// <summary>
        /// User name entered by user (from form, required)
        /// </summary>
        public string name { get; set; } = string.Empty;

        /// <summary>
        /// Comment message (from form, required)
        /// </summary>
        public string comment { get; set; } = string.Empty;

        /// <summary>
        /// Review id (required)
        /// </summary>
        public int review_id { get; set; }

        /// <summary>
        /// Article id (required)
        /// </summary>
        public int article_id { get; set; }

        /// <summary>
        /// User email (from form, required)
        /// </summary>
        public string email { get; set; } = string.Empty;

  
    }
}
