namespace DiplomaMarketBackend.Entity.Models
{
    public class OrderItemModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public OrderModel Order { get; set; }
        public int ArticleId { get; set; }
        public ArticleModel Article { get; set; }
        public int Quantity { get; set; }

    }
}
