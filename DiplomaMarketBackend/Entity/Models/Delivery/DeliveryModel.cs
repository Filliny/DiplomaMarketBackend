namespace DiplomaMarketBackend.Entity.Models.Delivery
{
    public class DeliveryModel
    {
        public int Id { get; set; }
        public TextContent? Name { get; set; }
        public string Description { get; set; } = string.Empty;

        public List<BranchModel> Branches { get; set; }

        public DeliveryModel() {
        
            Branches = new List<BranchModel>();
        }

    }
}
