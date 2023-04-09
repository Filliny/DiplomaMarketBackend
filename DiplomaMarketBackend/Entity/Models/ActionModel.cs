namespace DiplomaMarketBackend.Entity.Models
{
    public class ActionModel
    {
        public int Id { get; set; }
        public TextContent? Name { get; set; } 
        public TextContent? Description { get; set; }
        public List<ArticleModel> Articles { get; set; }
        public string BannerBig { get; set; }
        public string BannerSmall { get; set; }

        public ActionModel()
        {
            Articles = new();
        }
    }
}
