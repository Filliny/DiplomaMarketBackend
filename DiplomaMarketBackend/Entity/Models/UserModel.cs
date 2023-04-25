using DiplomaMarketBackend.Entity.Models;
using Microsoft.AspNetCore.Identity;
using System;

namespace Lessons3.Entity.Models
{
    public class UserModel: IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }
        public string? PreferredLanguageId { get; set; }
        public Language? PreferredLanguage { get; set; }
        public DateTime RegDate { get; set; }
        public DateTime BirthDay { get; set;}
    }
}