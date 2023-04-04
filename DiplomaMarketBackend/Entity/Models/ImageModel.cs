using DiplomaMarketBackend.Parser.Article;

namespace DiplomaMarketBackend.Entity.Models
{
    public class ImageModel
    {
        public int Id { get; set; }
        public PictureModel? original { get; set; }
        public PictureModel? base_action { get; set; }
        public PictureModel? preview { get; set; }
        public PictureModel small { get; set; }
        public PictureModel? medium { get; set; }
        public PictureModel? large { get; set; }
        public PictureModel? big_tile { get; set; }
        public PictureModel? big { get; set; }
        public PictureModel? mobile_medium { get; set; }
        public PictureModel? mobile_large { get; set; }

        public int? ArticleModelId { get; set; }
        public ArticleModel? ArticleModel { get; set; }
    }
}
