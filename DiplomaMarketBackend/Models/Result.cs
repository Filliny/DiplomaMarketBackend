namespace DiplomaMarketBackend.Models
{
    public class Result
    {
        public string Status { get; set; } = string.Empty;
        public string? Message { get; set; }
        public dynamic? Entity { get; set; }


    }
}
