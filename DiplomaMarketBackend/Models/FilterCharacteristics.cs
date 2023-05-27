namespace DiplomaMarketBackend.Models
{
    public class FilterCharacteristics
    {
        public class ListChracteristic{ 
         
            public int id { get; set; } 
            public string name { get; set; } = string.Empty;
        }

        public int category_id { get; set; }
        public string category_name { get; set; } = string.Empty;
        public List<ListChracteristic> chartacteristics { get; set; } = new List<ListChracteristic>();
    }
}
