using Microsoft.IdentityModel.Tokens;

namespace DiplomaMarketBackend.Helpers
{
    public enum BucketNames
    {
        logo,//for brands
        category,//for categories
        original,//for pictures ...
        base_action =240,
        preview=140,
        small=40,
        medium=80,
        large=200,
        big_tile=400,
        big=720,
        mobile_large=170,
        mobile_medium=100,
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
