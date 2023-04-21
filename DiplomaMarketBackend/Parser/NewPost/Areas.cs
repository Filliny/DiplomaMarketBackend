namespace DiplomaMarketBackend.Parser.NewPost.Areas
{
 
    public class Datum
    {
        public string Ref { get; set; }
        public string AreasCenter { get; set; }
        public string DescriptionRu { get; set; }
        public string Description { get; set; }
    }

    public class Root
    {
        public bool success { get; set; }
        public List<Datum> data { get; set; }
        public List<object> errors { get; set; }
        public List<object> warnings { get; set; }
        public List<object> info { get; set; }
        public List<object> messageCodes { get; set; }
        public List<object> errorCodes { get; set; }
        public List<object> warningCodes { get; set; }
        public List<object> infoCodes { get; set; }
    }

}
