using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Models;
using Microsoft.EntityFrameworkCore;

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


        public static TextContent CreateFromDictionary(BaseContext _db, Dictionary <string,string> translations, bool save = true)
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

            if(save) _db.SaveChanges();

            return textContent;

        }

        /// <summary>
        /// Updates text content with translations from LanguageId:Translation dictionary
        /// If key present in given content is absent in translations dictionary- then removed from content
        /// </summary>
        /// <param name="_db">Database context</param>
        /// <param name="content">Existing text content to update</param>
        /// <param name="translations">Translations dictionary</param>
        /// <param name="save">Is to  save changes on method exit? default= true</param>
        /// <exception cref="Exception"></exception>
        public static void UpdateFromDictionary(BaseContext _db, TextContent? content, Dictionary<string, string> translations, bool save = true)
        {
            if(content != null && content.Translations == null)
                content = _db.textContents.Include(c=>c.Translations).FirstOrDefault(c=>c.Id == content.Id);
            if (content == null)
                throw new Exception("TextContent null or not found!");

            //update main string if any changes
            if (!content.OriginalText.Equals(translations[content.OriginalLanguageId]))
                content.OriginalText = translations[content.OriginalLanguageId];

            ///update and create from incoming dictionary
            foreach(var new_translation in translations)
            {
                var exist_transl = content.Translations.FirstOrDefault(t => t.LanguageId == new_translation.Key);

                if (exist_transl != null)
                {
                    if(!exist_transl.TranslationString.Equals(new_translation.Value))
                        exist_transl.TranslationString = new_translation.Value;
                    _db.translations.Update(exist_transl);
                }
                else
                {
                    var new_string = new Translation
                    {
                        LanguageId = new_translation.Key,
                        TranslationString = new_translation.Value,
                    };
                    content.Translations.Add(new_string);
                }

            }

            //if existing translation key absent in income dectionary - we remove
            foreach(var exist_transl in content.Translations)
            {
                if (!translations.ContainsKey(exist_transl.LanguageId))
                {
                    content.Translations.Remove(exist_transl);
                    _db.translations.Remove(exist_transl);
                }
            }

            if (save) _db.SaveChanges();

        }

        public static void Delete(BaseContext _db, TextContent textContent)
        {
            if(textContent == null) return;

            if(textContent.Translations.Count == 0) // if u forget extract translations in outer request
                textContent.Translations  = _db.translations.Where(t=>t.TextContentId == textContent.Id).ToList();

            if (textContent.Translations.Count > 0) { 
            
                foreach(var translation in textContent.Translations)
                {
                    _db.translations.Remove(translation);
                }
            }
            _db.textContents.Remove(textContent);
        }
    }
}
