namespace DiplomaMarketBackend.Models
{
    public class Order
    {
        public class Receiver
        {
            public string name { get; set; } = string.Empty;
            public string last_name { get; set; } = string.Empty;
            public string middle_name { get; set; } = string.Empty;
            public string email { get; set; } = string.Empty;
        }

        public class OrderUser
        {
            public string name { get; set; } = string.Empty;
            public string last_name { get; set; } = string.Empty;
            public string phone { get; set; } = string.Empty;
            public string email { get; set; } = string.Empty;
        }

        public class PayData
        {
            public int payment_type_id { get; set; }
            public string? email { get; set; }
            public string? EDRPOU { get; set; }
            public string? legal_entity_name { get; set; }
            
        }

        public int id { get; set; }

        /// <summary>
        /// User Id
        /// </summary>
        public string?  user_id{ get; set; }

        /// <summary>
        /// User data for new user autoregistration (null if user logged/ignored if logged)
        /// </summary>
        public OrderUser? user { get; set; }

        /// <summary>
        /// Order receiver data - if emails equal user=recieiver
        /// </summary>
        public Receiver? receiver { get; set; }

        /// <summary>
        /// Payment data with payment type id and required fuileds
        /// </summary>
        public PayData? payData { get; set; } 

        /// <summary>
        /// goods dictionary, where key - article id, value - quantity
        /// </summary>
        public Dictionary<int , int> goods { get; set; } = new Dictionary<int , int>();

        /// <summary>
        /// List of certificates codes
        /// </summary>
        public List<string> certificates { get; set; } = new List<string>();

        /// <summary>
        /// List of promocodes codes
        /// </summary>
        public List<string> promo_codes { get; set; } = new List<string>();

        /// <summary>
        /// Id of delivery branch
        /// </summary>
        public int delivery_branch_id { get; set;}

        /// <summary>
        /// Order view summ for checking
        /// </summary>
        public decimal order_summ { get; set;}

    }
}
