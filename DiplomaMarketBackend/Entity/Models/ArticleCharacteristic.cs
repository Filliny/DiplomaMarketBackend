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

    public enum FilterType
    {
        checkbox,
        slider
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
        public int? Order { get; set; }
        public string? Comparable { get; set; }
        public CharacteristicType Type { get; set;}
        public List<ValueModel> Values { get;}

        public int ? GroupId { get; set; }
        public CharacteristicGroupModel? Group { get; set; }
        public FilterType filterType { get; set; }
        public bool show_in_filter { get; set; } = true;
        public int roz_har_id { get; set; }

        public ArticleCharacteristic()
        {
            Values = new();
        }

    }
}
