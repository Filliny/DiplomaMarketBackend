using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json;

namespace DiplomaMarketBackend.Models
{
    public class Category
    {
        /// <summary>
        /// Category id (can be null)
        /// </summary>
        public int? id { get; set; }

        /// <summary>
        /// Category names as serialized dictionary {"LanguageId":"CategoryNameLocalization"} (required)
        /// E.g. {'UK':'Газонокосарки'}
        /// </summary>
        public string names { get; set; } = string.Empty;

        /// <summary>
        /// Parent category id (can be null)
        /// </summary>
        public int? parent_id { get; set; }

        /// <summary>
        /// Show in additional category id
        /// </summary>
        public int? showin_category_id { get; set; }

        /// <summary>
        /// Icon file for root category (FormFile - null /required if parent Id null)
        /// </summary>
        [JsonIgnore]
        public IFormFile? root_icon { get; set; }

        /// <summary>
        /// Icon url for root category (null /required if parent Id null)
        /// </summary>
        public string? existing_icon { get; set; }

        /// <summary>
        /// Category image to show in subcategories (FormFile)
        /// </summary>
        [JsonIgnore]
        public IFormFile? category_image { get; set;}

        /// <summary>
        /// Category image url to show in subcategories
        /// </summary>
        public string? existing_image { get; set; }

        /// <summary>
        /// Does category active
        /// </summary>
        public bool? is_active { get; set; }

        /// <summary>
        /// Cildren categories for reference - can be null for create\update
        /// </summary>
        public List<dynamic>? children { get; set; }
    }
}
