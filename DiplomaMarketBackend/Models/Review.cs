namespace DiplomaMarketBackend.Models
{
    public class Review
    {
        /// <summary>
        /// Obligatory only on put (can not present)
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        ///User name entered by user (from form, required)
        /// </summary>
        public string name { get; set; } = string.Empty;

        /// <summary>
        /// Main review message (from form, required)
        /// </summary>
        public string comment { get; set; } = string.Empty;

        /// <summary>
        /// Star defined article rate from user 1 to 5  (from form, required)
        /// </summary>
        public int rate { get; set; }

        /// <summary>
        /// Article id (required)
        /// </summary>
        public int article_id { get; set; }

        /// <summary>
        /// User described good article values (from form, required)
        /// </summary>
        public string pros { get; set; } = string.Empty;

        /// <summary>
        /// User described bad article sides (from form, required)
        /// </summary>
        public string cons { get; set; } = string.Empty;

        /// <summary>
        /// User email (from form, required)
        /// </summary>
        public string email { get; set; } = string.Empty;

        /// <summary>
        /// User video url (from form, can be null)
        /// </summary>
        public string? video_url { get; set; } = string.Empty;

        /// <summary>
        /// Is user want get emails on answers (from form, can be null)
        /// </summary>
        public bool? get_email_on_answers { get; set; }

        /// <summary>
        /// User photos of article (from form, can not present)
        /// </summary>
        public List<IFormFile> images { get; set; }

        /// <summary>
        /// Only for admin form (can not present for new review)
        /// </summary>
        public bool? review_approved { get; set; }

        /// <summary>
        /// Time of posting (can be absent or null)
        /// </summary>
        public DateTime? date_time { get; set; }

        public Review()
        {
            images = new();
        }
    }
}
