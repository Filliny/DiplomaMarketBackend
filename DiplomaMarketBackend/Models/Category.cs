namespace DiplomaMarketBackend.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public int? ParentId { get; set; }

        public List<Category> Children { get; set; }

        public Category()
        {
            Children = new();
        }
    }
}
