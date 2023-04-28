using DiplomaMarketBackend.Entity.Models;

namespace DiplomaMarketBackend.Entity.Models
{
    public class ArticleModel
    {
        public int Id { get; set; }

        public int TitleId { get; set; }
        public TextContent Title { get; set; }

        public int DescriptionId { get; set; }
        public TextContent Description { get; set; }


        public int  DocketId { get; set; }
        public TextContent Docket { get; set; }

        public decimal Price { get; set; }
        public decimal OldPrice { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Updated { get; set; }
        public int CategoryId { get; set; }
        public CategoryModel Category { get; set; }
        public int? TopCategoryId { get; set; }
        public CategoryModel? TopCategory { get; set; }
        public int? BrandId { get; set; }
        public BrandModel? Brand { get; set; }
        public string? Status { get; set; } //active/inactive
        public string? SellStatus { get; set; }//available/unavailable
        public List<WarningModel> Warning { get; set; }
        public List<VideoModel> Video { get; set; }
        public List<ImageModel> Images { get; set; }
        public List<BreadcrumbsModel> Breadcrumbs { get; set; }
        public List<ValueModel> CharacteristicValues { get; set; }
        public List<TagModel> Tags { get; set; }
        public List<ActionModel> Actions { get; set; }
        public int rztk_art_id { get; set; }
        public decimal RatingCalculated { get; set; }

        public List<ReviewModel> Reviews { get; set; }

        public int? SellerId { get; set; }
        public SellerModel? Seller { get; set; }


        public ArticleModel()
        {
            CharacteristicValues = new ();
            Video = new List<VideoModel>();
            Images = new List<ImageModel>();
            Breadcrumbs = new List<BreadcrumbsModel>();
            Tags = new();
            Warning = new();
            Actions = new();

        }

    }
}
