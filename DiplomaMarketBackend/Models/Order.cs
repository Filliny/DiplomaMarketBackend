using System.ComponentModel.DataAnnotations;

namespace DiplomaMarketBackend.Models
{
    public class Order
    {
        /// <summary>
        /// Order receiver (fill if receiver different then user)
        /// </summary>
        public class Receiver
        {
            public int Id { get; set; }
            
            [Required(ErrorMessage = "first name is required")]
            public string first_name { get; set; } = string.Empty;
            
            [Required(ErrorMessage = "last name is required")]
            public string last_name { get; set; } = string.Empty;
            public string middle_name { get; set; } = string.Empty;
            public string? profile_name { get; set;} 
            
            [Required(ErrorMessage = "Email is required")]
            public string email { get; set; } = string.Empty;
            public string? phone { get; set; } = string.Empty;
        }

        /// <summary>
        /// Order User for registration form 
        /// </summary>
        public class OrderUser
        {
            [Required(ErrorMessage = "first name is required")]
            public string first_name { get; set; } = string.Empty;
            
            [Required(ErrorMessage = "last name is required")]
            public string last_name { get; set; } = string.Empty;
            public string phone { get; set; } = string.Empty;
            
            [Required(ErrorMessage = "Email is required")]
            public string email { get; set; } = string.Empty;
        }

        /// <summary>
        /// Payment data 
        /// </summary>
        public class PayData
        {
            [Required(ErrorMessage = "payment_type_id is required")]
            public int payment_type_id { get; set; }
            public string? email { get; set; }
            public string? EDRPOU { get; set; }
            public string? legal_entity_name { get; set; }
            
        }

        /// <summary>
        /// Order items for order list
        /// </summary>
        public class Item
        {
            [Required(ErrorMessage = "id is required")]
            public int article_id { get; set; }
            
            [Required(ErrorMessage = "price is required")]
            public decimal price { get; set; }
            
            [Required(ErrorMessage = "quantity is required")]
            public int quantity { get; set; }
            
            public string? title { get; set; }
            
            public string? small_img { get; set; }

        }

        //entity serializes into db string 
        public class DeliveryAdress
        {
            public string city_name { get; set; } = string.Empty;
            public string street { get; set; } = string.Empty;
            public string building { get; set; } = string.Empty;
            public string apartment { get; set; } = string.Empty;
        }

        /// <summary>
        /// Order status in one of next states:
        /// New, PaymentPending, Paid, InProcess, Shipped, Delivered,Cancelled
        /// </summary>
        public string? status { get; set; }

        /// <summary>
        /// Order Id
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// User Id
        /// </summary>
        public string? user_id{ get; set; }

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
        [Required(ErrorMessage = "payData is required")]
        public PayData payData { get; set; } = new PayData();

        /// <summary>
        /// List of order items (articles, goods)
        /// </summary>
        [Required(ErrorMessage = "goods is required")]
        public List<Item> goods { get; set; } = new List<Item>();

        /// <summary>
        /// List of certificates codes
        /// </summary>
        public List<string>? certificates { get; set; }

        /// <summary>
        /// List of promocodes codes
        /// </summary>
        public List<string>? promo_codes { get; set; }

        /// <summary>
        /// Id of delivery branch
        /// </summary>
        [Required(ErrorMessage = "delivery_branch_id is required")]
        public int delivery_branch_id { get; set;}

        /// <summary>
        /// Delivery adress for curier delivery if selected
        /// </summary>
        public DeliveryAdress? delivery_adress { get; set; }

        /// <summary>
        /// Order view summ for checking
        /// </summary>
        [Required(ErrorMessage = "total_price is required")]
        public decimal total_price { get; set;}
        
        public string? create_date { get; set; }

    }
}
