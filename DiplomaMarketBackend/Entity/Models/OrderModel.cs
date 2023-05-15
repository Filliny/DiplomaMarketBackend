using DiplomaMarketBackend.Entity.Models.Delivery;
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

        public string? UserId { get; set; }
        public UserModel? User { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set;}

        public decimal TotalPrice { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.New;

        public List<OrderItemModel> Items { get; set; }

        public List<PromoCodeModel> PromoCodes { get; set; }
        public List<CertificateModel> Certificates { get; set; }

        public int? ReceiverId { get; set; }
        public ReceiverModel? Receiver { get; set; }

        public int? PaymentTypeId { get; set; }
        public PaymentTypesModel? PaymentType { get; set; }
        public string PaymentData { get; set; }  = string.Empty;

        public int? DeliveryBranchId { get; set; }
        public BranchModel? DeliveryBranch { get; set; }

        public OrderModel()
        {
            Items = new();
            Certificates = new();
            PromoCodes = new();
        }

    }
}
