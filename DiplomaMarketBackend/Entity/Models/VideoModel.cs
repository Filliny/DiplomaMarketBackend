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
    }
}
