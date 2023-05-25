namespace DiplomaMarketBackend.Entity.Models
{
    public class ValueModel
    {
        public int Id { get; set; }

        public int? TitleId { get; set; }
        public TextContent? Title { get; set; }

        public string? href { get; set; }

        public int CharacteristicTypeId { get; set; }
        public ArticleCharacteristic CharacteristicType { get; set; }
        public List<ArticleModel> Articles { get; set; }

        public ValueModel()
        {
            Articles = new();
        }
    }
}
