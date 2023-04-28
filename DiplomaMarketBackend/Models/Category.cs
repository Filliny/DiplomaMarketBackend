namespace DiplomaMarketBackend.Models
{
    public class Category
    {
        /// <summary>
        /// Category id (can be null)
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Category names as serialized dictionary {"LanguageId":"CategoryNameLocalization"} (required)
        /// E.g. {'UK':'Газонокосарки'}
        /// </summary>
        public string Names { get; set; } = string.Empty;

        /// <summary>
        /// Parent category id (can be null)
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// Show in additional category id
        /// </summary>
        public int? ShowInCategoryId { get; set; }

        /// <summary>
        /// Icon file for root category (null /required if parent Id null)
        /// </summary>
        public IFormFile? RootIconFile { get; set; }

        /// <summary>
        /// Category image to show in subcategories
        /// </summary>
        public IFormFile? CategoryImage { get; set;}



    }
}
