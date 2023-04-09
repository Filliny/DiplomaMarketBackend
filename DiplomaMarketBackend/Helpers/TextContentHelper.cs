using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Models;

namespace DiplomaMarketBackend.Helpers
{
    public static class TextContentHelper
    {
        public static void UpdateTextContent(BaseContext _db, string text, int? content_id, string lang_id)
        {
            var textContent = _db.textContents.FirstOrDefault(c => c.Id == content_id);
            if (textContent == null) return;

            var exist_transl = _db.translations.FirstOrDefault(t => t.TextContentId == content_id && t.LanguageId == lang_id);

            if (exist_transl != null)
            {
                exist_transl.TranslationString = text;
                _db.Update(exist_transl);
            }
            else
            {
                var translation = new Translation()
                {
                    LanguageId = lang_id.ToUpper(),
                    TranslationString = text,
                };

                textContent.Translations.Add(translation);

            }


        }

        public static TextContent? CreateTextContent(BaseContext _db, string text, string lang_id)
        {

            var translation = new Translation()
            {
                LanguageId = lang_id.ToUpper(),
                TranslationString = text,
            };

            _db.translations.Add(translation);


            var textContent = new TextContent()
            {
                OriginalLanguageId = lang_id.ToUpper(),
                OriginalText = text,
            };

            textContent.Translations.Add(translation);

            _db.textContents.Add(textContent);

            return textContent;

        }
    }
}
