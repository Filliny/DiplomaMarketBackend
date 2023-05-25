using DiplomaMarketBackend.Entity.Models;
using Mapster;
using System;
using System.Reflection;

namespace DiplomaMarketBackend.Mappings
{
    public static class MapsterConfig
    {
        public static void RegisterMapsterConfiguration(this IServiceCollection services)
        {
            //TypeAdapterConfig<TextContent, IDictionary<string, string>>
            //   .NewConfig()
            //   .Map(dest => dest, src => src.Translations.ToDictionary(t => t.LanguageId ?? "", t => t.TranslationString));

            //TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

        }
    }
}
