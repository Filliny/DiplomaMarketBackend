namespace DiplomaMarketBackend.Entity.Models
{
    public class PromoCodeModel
    {
        public int Id { get; set; }
        public string PromoCode { get; set; } = string.Empty;
        public decimal Summ { get; set; }
        public DateTime Issued { get; set; }
        public DateTime ValidUntil { get; set; }
        public DateTime? Closed { get; set; } = null;
        public List<OrderModel>? Order { get; set; }
    }
}
