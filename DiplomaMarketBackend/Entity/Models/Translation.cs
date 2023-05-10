namespace DiplomaMarketBackend.Entity.Models
{
    public class Translation
    {
        public int Id { get; set; }
        public string? TranslationString { get; set; } 
        public Language? Language { get; set; }
        public string? LanguageId { get; set; } 
        public TextContent? TextContent { get; set; }
        public int? TextContentId { get; set;}

    }
}
