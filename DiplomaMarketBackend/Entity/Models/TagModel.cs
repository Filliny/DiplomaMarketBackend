namespace DiplomaMarketBackend.Entity.Models
{
    public class TagModel
    {
        public int Id { get; set; }

        public int? NameId { get; set; }
        public TextContent? Name { get; set; }

        public int? TitleId { get; set; }
        public TextContent? Title { get; set; }
        public int? Priority { get; set; }
    }
}
