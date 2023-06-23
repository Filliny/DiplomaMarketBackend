namespace DiplomaMarketBackend.Models
{
    /// <summary>
    /// Entitiy used mostly in return
    /// </summary>
    public class UserFull:User
    {
        public string? Id { get; set; }
        public string? first_name { get; set; } = string.Empty;
        public string? last_name { get; set; } = string.Empty;
        public string? middle_name { get; set; } = string.Empty;
        public string? preferred_language_id { get; set; }
        
        public DateTime reg_date { get; set;}
        public DateTime birth_day { get; set; }
        public string? phone_number { get; set; } = string.Empty;
        public int? customer_group_id { get; set; }

        public IList<string> roles { get ; set; } = new List<string>();

        public bool email_notify { get; set; } = true;
        public int gender { get; set; }

    }
}
