namespace DiplomaMarketBackend.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public int? ParentId { get; set; }

        public string? SmallIcon { get; set; }

        public string? BigPicture { get; set; }

        public List<Category> Children { get; set; }

        public Category()
        {
            Children = new();
        }
    }
}
