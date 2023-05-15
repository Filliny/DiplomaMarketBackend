namespace DiplomaMarketBackend.Entity.Models
{
    public class PaymentTypesModel
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public PaymentTypesModel? Parent { get; set; }
        public TextContent? Name { get; set; }
        public TextContent? Description { get; set; }
        public string URL { get; set; } = string.Empty;
        public string CallbackURL { get; set; } = string.Empty;
        public List<PaymentTypesModel> Childs { get; set; } = new();

    }
}
