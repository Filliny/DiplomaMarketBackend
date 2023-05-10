using AutoMapper;
using DiplomaMarketBackend.Entity.Models;

namespace DiplomaMarketBackend.Mappings
{
    public class TextContentToDictionamryMapping:IMapWith<TextContent>
    {
        public void Mapping(Profile profile)
        {
            profile.CreateMap<TextContent, Dictionary<string, string>>().AfterMap((s, d) =>
            {
                foreach(var item in s.Translations) {
                
                    if(item != null)
                    {
                        d.Add(item.LanguageId, item.TranslationString);
                    }
                }
            });
        }
    }
}
