using AutoMapper;
using DiplomaMarketBackend.Entity.Models;
using DiplomaMarketBackend.Helpers;
using DiplomaMarketBackend.Mappings;
using Microsoft.EntityFrameworkCore;

namespace DiplomaMarketBackend.Models
{
    public class ArticleDTO:IMapWith<ArticleModel>
    {
        public class PreviewImage: IMapWith<ImageModel>
        {
            public int id { get; set; }
            public string url { get; set; }

            public void Mapping(Profile profile)
            {
                profile.CreateMap<ImageModel, PreviewImage>()
                    .ForMember(dto => dto.id,
                        opt => opt.MapFrom(img => img.Id))
                    .ForMember(dto => dto.url,
                        opt => opt.MapFrom(img => img.preview.url)).AfterMap<SetImgFullURL>();
                
            }
        }

        public class TypeValue
        {
            public int value_id { get; set; }
            public string value_name { get; set; } = string.Empty;
            public int charcteristic_id { get; set; }
            public string charcteristic_name { get; set; } = string.Empty;
        }

        public class Warning:IMapWith<WarningModel>
        {
            public int id { get; set; }
            public Dictionary<string, string> message { get; set; }

            public void Mapping(Profile profile)
            {
                profile.CreateMap<WarningModel, Warning>();
                  
                    //.ForMember(dto => dto.id,
                    //    opt => opt.MapFrom(warn => warn.Message.Translations))
                    //.ForMember(dto => dto.url,
                    //    opt => opt.MapFrom(img => img.preview.url))
                    //.ConvertUsing((s,v,d) =>
                    //{
                    //    foreach (var trans in s.Message.Translations)
                    //    {

                    //    }

                    //    return new Dictionary<string, string>();
                    //});
            }
        }

        /// <summary>
        /// Article id
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Article title translations in dictionary "locale_id":"translation"
        /// </summary>
        public Dictionary<string,string> title { get; set; } = new Dictionary<string,string>();

        /// <summary>
        /// Article description translations in dictionary "locale_id":"translation"
        /// </summary>
        public Dictionary<string, string> description { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Article docket translations in dictionary "locale_id":"translation"
        /// </summary>
        public Dictionary <string, string> docket { get; set;} = new Dictionary<string, string>();

        /// <summary>
        /// Current price
        /// </summary>
        public decimal price { get; set; }

        /// <summary>
        /// Old price
        /// </summary>
        public decimal old_price { get; set; }

        /// <summary>
        /// Category Id
        /// </summary>
        public int category_id { get; set; }

        /// <summary>
        /// Brand Id
        /// </summary>
        public int brand_id { get; set; }

        /// <summary>
        /// Status (active/inactive)
        /// </summary>
        public string status { get; set;} = "active";

        /// <summary>
        /// Selling status (available/unavailable)
        /// </summary>
        public string sell_status { get; set; } = "available";

        /// <summary>
        /// Article warnings List - each with translations in dictionary "locale_id":"translation"
        /// </summary>
        public List<Warning> warning { get; set; } = new List<Warning>();

        /// <summary>
        /// Url strings of videos
        /// </summary>
        public List<string> video_urls { get; set; } = new List<string>();

        /// <summary>
        /// images entities with image id and preview url for admin
        /// </summary>
        public List<PreviewImage> images { get; set; } = new List<PreviewImage>();

        /// <summary>
        /// Article attributes (characterisitc values) list with characterisitc name and id
        /// </summary>
        public List<TypeValue> values { get; set; } = new List<TypeValue>();

        /// <summary>
        /// List of actions ids
        /// </summary>
        public List<int> actions_ids { get; set; } = new List<int>();

        /// <summary>
        /// Seller id
        /// </summary>
        public int seller_id { get; set; } = 1;

        public HttpRequest? req { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ArticleModel, ArticleDTO>();

        }

    }
}
