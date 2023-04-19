namespace DiplomaMarketBackend.Entity.Models
{
    public class SellerModel
    { 
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;    
        public string Description { get; set; } = string.Empty; 
        public decimal Rate { get; set; }
        public List<ArticleModel> Articles { get; set; }

        public SellerModel() {
        
            Articles = new List<ArticleModel>();
        }
    }
}
