using System.ComponentModel.DataAnnotations;

namespace DiplomaMarketBackend.Models
{
    public class User
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? user_name { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? password { get; set; }
    }
}
