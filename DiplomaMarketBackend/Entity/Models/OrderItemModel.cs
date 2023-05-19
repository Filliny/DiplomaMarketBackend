using System.Text.Json.Serialization;

namespace DiplomaMarketBackend.Entity.Models
{
    public class OrderItemModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        [JsonIgnore]
        public OrderModel Order { get; set; }
        public int ArticleId { get; set; }
        public ArticleModel Article { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

    }
}
