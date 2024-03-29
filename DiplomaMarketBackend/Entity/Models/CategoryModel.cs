﻿namespace DiplomaMarketBackend.Entity.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }

        public int? NameId { get; set; }
        public TextContent Name { get; set; }

        public int? DescriptionId { get; set; }
        public TextContent Description { get; set; }
        public byte[]? ImgData { get; set; }
        public string? ImgUrl { get; set; }

        public int? ParentCategoryId { get; set; }
        public CategoryModel? ParentCategory { get; set; }
        public List<CategoryModel> ChildCategories { get; set; }
        public int? rztk_cat_id { get; set; }

        public bool ? is_active { get; set; } = true;

        public int ? ShowInCategoryId { get; set; }

        public List<BrandModel> Brands { get; set; }



        public CategoryModel() { 
        
            ChildCategories = new List<CategoryModel>();
           
        }
    }
}
