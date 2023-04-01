namespace DiplomaMarketBackend.Entity.Models
{
    public enum CharacteristicType {
        ComboBox,
        ListValues,
        Decimal,
        TextInput,
        TextArea,
        List
    }

    public class ArticleCharacteristic
    {
        public int Id { get; set; }

        public int? TitleId { get; set; }
        public TextContent? Title { get; set; }

        public int? NameId { get; set; }
        public TextContent? Name { get; set; } //on eng on rztk

        public string? Status { get; set; }
        public int? CategoryId { get; set; }
        public CategoryModel? Category { get; set; }
        public int? ArticleId { get; set; }
        public ArticleModel? Article { get; set; }
        public CharacteristicType Type { get; set;}
        public List<CharacteristicValueModel> Values { get;}


        public ArticleCharacteristic()
        {
            Values = new();
        }

    }
}
