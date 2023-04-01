namespace DiplomaMarketBackend.Entity.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }

        public int? NameId { get; set; }
        public TextContent Name { get; set; }

        public int? DescriptionId { get; set; }
        public TextContent Description { get; set; }

        public int? ParentCategoryId { get; set; }
        public CategoryModel? ParentCategory { get; set; }
        public List<CategoryModel> ChildCategories { get; set; }
        public int? rztk_cat_id { get; set; }

        public CategoryModel() { 
        
            ChildCategories = new List<CategoryModel>();
        }
    }
}
