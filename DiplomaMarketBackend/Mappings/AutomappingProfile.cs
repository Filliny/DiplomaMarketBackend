using AutoMapper;
using DiplomaMarketBackend.Entity.Models;
using DiplomaMarketBackend.Models;
using System;

namespace DiplomaMarketBackend.Mappings
{
    public class AutomappingProfile:Profile
    {
        public AutomappingProfile()
        {
            CreateMap<ArticleModel, Article>();
        }
    }
}
