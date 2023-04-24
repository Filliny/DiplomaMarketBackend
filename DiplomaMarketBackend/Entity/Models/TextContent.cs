namespace DiplomaMarketBackend.Entity.Models
{
    public class TextContent
    {
        public int Id { get; set; }
        public string? OriginalText { get; set; }
        public Language? OriginalLanguage { get; set; }
        public string? OriginalLanguageId { get; set; }
        public List<Translation> Translations { get; set; }

        public TextContent()
        {
            Translations = new List<Translation>();
        }

        public string Content(string language)
        {
            var translation = Translations.FirstOrDefault(t=>t.LanguageId == language.ToUpper());


            if(translation == null) {

                return OriginalText??"";
            }

            return translation.TranslationString??"";
        }

    }
}
