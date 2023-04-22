namespace DiplomaMarketBackend.Parser.Delengine.Branches
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Company
    {
        public string uuid { get; set; }
        public string name_uk { get; set; }
    }

    public class Datum
    {
        public string uuid { get; set; }
        public object code { get; set; }
        public int number { get; set; }
        public object weight_limit { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public int status { get; set; }
        public string address_uk { get; set; }
        public object address_ru { get; set; }
        public object address_en { get; set; }
        public Settlement settlement { get; set; }
        public Company company { get; set; }
        public DepartmentType department_type { get; set; }
        public string schedules { get; set; }
    }

    public class DepartmentType
    {
        public string uuid { get; set; }
        public string name_uk { get; set; }
    }

    public class Paginate
    {
        public int total { get; set; }
        public int per_page { get; set; }
        public int current_page { get; set; }
    }

    public class Root
    {
        public string status { get; set; }
        public List<Datum> data { get; set; }
        public Paginate paginate { get; set; }
    }

    public class Settlement
    {
        public string uuid { get; set; }
        public string name_uk { get; set; }
    }


}
