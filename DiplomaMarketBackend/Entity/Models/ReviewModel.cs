using Lessons3.Entity.Models;

namespace DiplomaMarketBackend.Entity.Models
{
    public enum ReviewType
    {
        review,
        answer
    }

    public enum ReviewSort
    {
        from_buyer,
        by_date,
        helpful,
        with_attach
    }

    public class ReviewModel
    {
        public int Id { get; set; }

        public string? UserId { get; set; }
        public UserModel? User { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public Boolean GetEmailOnAnswers { get; set; }

        public int? ReviewId { get; set; }
        public ReviewModel? Review { get; set; }

        public ReviewType Type { get; set; } = ReviewType.review;

        public int? ArticleId { get; set; }
        public ArticleModel? Article { get; set; }

        public List<ReviewModel> Answers { get; set; }

        public string Pros { get; set; } = string.Empty;
        public string Cons { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public short Rate { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public string VideoUrl { get; set; } = string.Empty;

        public bool ReviewApproved { get; set; }

        public List<string> UserImages { get; set; }


        public DateTime DateTime { get; set; } = DateTime.Now;

        public ReviewModel()
        {
            Answers = new();
            UserImages = new();
        }
    }
}
