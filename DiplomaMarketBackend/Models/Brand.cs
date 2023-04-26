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
        public string name { get; set; }

        /// <summary>
        /// Brand logo
        /// </summary>
        public IFormFile? logo { get; set; }
            
    }
}
