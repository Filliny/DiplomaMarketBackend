using System.ComponentModel.DataAnnotations;

namespace DiplomaMarketBackend.Models
{
    public class Brand
    {
        public int? id { get; set; }

        /// <summary>
        /// Brand name (required)
        /// </summary>
        [Required(ErrorMessage = "Brand Name is required")]
        public string name { get; set; } = string.Empty;

        /// <summary>
        /// Brand logo FormFile from form
        /// </summary>
        public IFormFile? logo { get; set; }
            
    }
}
