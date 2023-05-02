using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Models;

namespace DiplomaMarketBackend.Helpers
{
    /// <summary>
    /// Helper for set and get string values for translated strings in entities
    /// </summary>
    public static class TextContentHelper
    {

        /// <summary>
        /// Update existing text content by adding new translation entity
        /// </summary>
        /// <param name="_db">database context</param>
        /// <param name="text">text value</param>
        /// <param name="content_id">existing content id </param>
        /// <param name="lang_id">language id</param>
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

        /// <summary>
        /// Create text content entity for translated values 
        /// (e.g. names, descriptions)
        /// </summary>
        /// <param name="_db">database context</param>
        /// <param name="text">default text value</param>
        /// <param name="lang_id">language id</param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates and Saves text content for uk and ru locales with UK main locale
        /// </summary>
        /// <param name="_db"></param>
        /// <param name="text_ua"></param>
        /// <param name="text_ru"></param>
        /// <returns></returns>
        public static TextContent? CreateFull(BaseContext _db, string text_ua, string text_ru)
        {

            var translation_ua = new Translation()
            {
                LanguageId = "UK",
                TranslationString = text_ua,
            };

            _db.translations.Add(translation_ua);

            var translation_ru = new Translation()
            {
                LanguageId = "RU",
                TranslationString = text_ru,
            };

            _db.translations.Add(translation_ru);


            var textContent = new TextContent()
            {
                OriginalLanguageId = "UK",
                OriginalText = text_ua,
            };

            textContent.Translations.Add(translation_ua);
            textContent.Translations.Add(translation_ru);

            _db.textContents.Add(textContent);

            _db.SaveChanges();

            return textContent;

        }


        public static TextContent? CreateFromDictionary(BaseContext _db, Dictionary <string,string> translations)
        {
            if (!translations.ContainsKey("UK")) throw new Exception("Dictionary for content creation lacks default locale 'UK' !");

            var textContent = new TextContent()
            {
                OriginalLanguageId = "UK",
              
            };

            foreach (var translation in translations)
            {
                if (!_db.Languages.Any(l => l.Id == translation.Key.ToUpper())) continue;

                if(translation.Key.ToUpper() == "UK") textContent.OriginalText = translation.Value;

                var new_translation = new Translation()
                {
                    LanguageId = translation.Key.ToUpper(),
                    TranslationString = translation.Value,
                };

                _db.translations.Add(new_translation);
                textContent.Translations.Add(new_translation);
            }

            _db.textContents.Add(textContent);

            _db.SaveChanges();

            return textContent;

        }

        public static void Delete(BaseContext _db, TextContent textContent)
        {
            if(textContent == null) return;
            if(textContent.Translations.Count > 0) { 
            
                foreach(var translation in textContent.Translations)
                {
                    _db.translations.Remove(translation);
                }
            }

            _db.textContents.Remove(textContent);
            _db.SaveChanges();
        }
    }
}
