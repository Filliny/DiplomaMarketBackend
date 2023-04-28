namespace DiplomaMarketBackend.Models
{
    public class OutCategory
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public int? ParentId { get; set; }

        public string? SmallIcon { get; set; }

        public string? BigPicture { get; set; }

        public List<OutCategory> Children { get; set; }

        public OutCategory()
        {
            Children = new();
        }
    }
}
