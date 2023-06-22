using System.ComponentModel.DataAnnotations;

namespace DiplomaMarketBackend.Models
{
    public class UserContacts
    {
        [Required(ErrorMessage = "Phone is required")]
        public string phone { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email is required")]
        public string email { get; set; } = string.Empty;
    }
}
