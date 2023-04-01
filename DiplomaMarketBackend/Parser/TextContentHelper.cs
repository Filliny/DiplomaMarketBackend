using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Models;

namespace DiplomaMarketBackend.Parser
{
    public static class TextContentHelper
    {
        public static void UpdateTextContent(BaseContext _db, string text, int content_id, string lang_id)
        {
            var textContent  = _db.textContents.FirstOrDefault(c=>c.Id == content_id);

            if (textContent == null) return;

            var translation = new Translation()
            {
                LanguageId = lang_id.ToUpper(),
                TranslationString = text,
            };

            textContent.Translations.Add(translation);

            _db.SaveChanges();

        }

        public static TextContent? CreateTextContent(BaseContext _db, string text, string lang_id)
        {

            var translation = new Translation()
            {
                LanguageId = lang_id.ToUpper(),
                TranslationString = text,
            };

            _db.translations.Add(translation);
            _db.SaveChanges();


            var textContent = new TextContent()
            {
                OriginalLanguageId = lang_id.ToUpper(),
                OriginalText = text,    
            };

            textContent.Translations.Add(translation);

            _db.textContents.Add(textContent);
            _db.SaveChanges();

            return textContent; 

        }
    }
}
