using Microsoft.IdentityModel.Tokens;

namespace DiplomaMarketBackend.Helpers
{
    public enum BucketNames
    {
        logo,//for brands
        category,//for categories
        original,//for pictures ...
        base_action,
        preview,
        small,
        medium,
        large,
        big_tile,
        big,
        mobile_large,
        mobile_medium,
        review
    }

    public static class UrlHelper
    {
        public static string GetImageURL(this HttpRequest Request, string bucketName, string Id)
        {
            if (Id.IsNullOrEmpty()) return null;
            return Request.Scheme + "://" + Request.Host + $"/api/Goods/{bucketName}/{Id}.jpg";
        }

        public static string GetImageURL(this HttpRequest Request, BucketNames bucketName, string Id)
        {
            if (Id.IsNullOrEmpty()) return null;

            return Request.Scheme + "://" + Request.Host + $"/api/Goods/{bucketName.ToString()}/{Id}.jpg";
        }


    }
}
