using DiplomaMarketBackend.Entity.Models;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace DiplomaMarketBackend.Entity.Models
{
    public enum Gender
    {
        unspecified,
        male,
        female
    }

    public class UserModel: IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }
        public string? PreferredLanguageId { get; set; }
        public Language? PreferredLanguage { get; set; }
        public DateTime RegDate { get; set; }
        public DateTime BirthDay { get; set;}
        public string DeliveryAddress { get; set; } = string.Empty;
        public Gender? Gender { get; set; } = Models.Gender.unspecified;
        public int? CustomerGroupId { get; set; }
        public CustomerGroupModel? CustomerGroup { get; set; }

        public List<ArticleModel> Favorites { get; set; } = new List<ArticleModel>();

        [JsonIgnore]
        public List<ReceiverModel> receivers { get; set; } = new();
    }
}