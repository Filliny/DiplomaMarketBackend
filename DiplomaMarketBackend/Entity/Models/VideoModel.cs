using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using System.Text.RegularExpressions;
using System.Web;

namespace DiplomaMarketBackend.Entity.Models
{
    public class VideoModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? URL { get; set; }
        public string? Type { get; set; }
        public string? PreviewURL { get; set; }
        public string? ExternalId { get; set; }//id on service youtube e.g.
        public string? Order { get; set; } //TODO change to order entity 

        public VideoModel()
        {
            
        }

        public VideoModel(string youtube_url)
        {
            var uri = new Uri(youtube_url);
            var query = HttpUtility.ParseQueryString(uri.Query);
            var id = query["v"];

            Title = "";
            URL = youtube_url;
            Type = "youtube";
            PreviewURL = String.Empty;
            ExternalId = id;

        }
    }
}
