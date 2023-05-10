using AutoMapper;
using DiplomaMarketBackend.Entity.Models;
using DiplomaMarketBackend.Helpers;
using DiplomaMarketBackend.Models;

namespace DiplomaMarketBackend.Mappings
{
    public class SetImgFullURL : IMappingAction<ImageModel, ArticleDTO.PreviewImage>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SetImgFullURL(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public void Process(ImageModel source, ArticleDTO.PreviewImage destination, ResolutionContext context)
        {
            destination.url = _httpContextAccessor.HttpContext.Request.GetImageURL("preview",destination.url);
        }
    }
}
