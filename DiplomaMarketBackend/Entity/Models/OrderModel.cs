using Lessons3.Entity.Models;

namespace DiplomaMarketBackend.Entity.Models
{
    public enum OrderStatus
    {
        New,
        PaymentPending,
        Paid,
        InProcess,
        Shipped,
        Delivered

    }

    public class OrderModel
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public UserModel? User { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set;}

        public decimal TotalPrice { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.New;

        public List<OrderItemModel> Items { get; set; }


        public OrderModel()
        {
            Items = new();
        }



    }
}
