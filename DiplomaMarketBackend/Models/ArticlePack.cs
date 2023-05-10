namespace DiplomaMarketBackend.Models
{
    public class ArticlePack
    {
        public string article_json { get; set; } = string.Empty; 
        public List<IFormFile> images { get; set; } = new List<IFormFile>();
    }
}
