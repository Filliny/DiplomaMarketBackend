namespace DiplomaMarketBackend.Parser.Mist.OneBranch
{
    public class City
    {
        public string ua { get; set; }
        public string ru { get; set; }
        public string en { get; set; }
    }

    public class District
    {
        public string ua { get; set; }
        public string ru { get; set; }
        public string en { get; set; }
    }

    public class Limits
    {
        public int receiving_only { get; set; }
        public int sending_only { get; set; }
        public int format_limit { get; set; }
        public int receiving_phone_reqired { get; set; }
        public int cash_pay_unavailible { get; set; }
        public int cash_terminal_availible { get; set; }
        public int place_max_kg { get; set; }
        public int parcel_max_kg { get; set; }
        public double parcel_max_m3 { get; set; }
        public int max_insurance { get; set; }
        public int max_places { get; set; }
        public int max_length { get; set; }
        public int max_width { get; set; }
        public int max_height { get; set; }
        public int free_storage_days { get; set; }
    }

    public class Region
    {
        public string ua { get; set; }
        public string ru { get; set; }
        public string en { get; set; }
    }

    public class Result
    {
        public string br_id { get; set; }
        public City city { get; set; }
        public string city_id { get; set; }
        public District district { get; set; }
        public string district_id { get; set; }
        public string lat { get; set; }
        public Limits limits { get; set; }
        public string lng { get; set; }
        public string location_description { get; set; }
        public string num { get; set; }
        public string num_showcase { get; set; }
        public Region region { get; set; }
        public string region_id { get; set; }
        public Street street { get; set; }
        public string street_id { get; set; }
        public string street_number { get; set; }
        public string type_id { get; set; }
        public TypePublic type_public { get; set; }
        public string working_hours { get; set; }
        public WorkingHoursDetailed working_hours_detailed { get; set; }
        public string zip { get; set; }
    }

    public class Root
    {
        public int status { get; set; }
        public object msg { get; set; }
        public List<Result> result { get; set; }
    }

    public class Street
    {
        public string ua { get; set; }
        public string ru { get; set; }
        public string en { get; set; }
    }

    public class TypePublic
    {
        public string ua { get; set; }
        public string ru { get; set; }
        public string en { get; set; }
    }

    public class WorkingHoursDetailed
    {
        public int mo { get; set; }
        public string mo_s { get; set; }
        public string mo_e { get; set; }
        public string mo_lb_s { get; set; }
        public string mo_lb_e { get; set; }
        public int tu { get; set; }
        public string tu_s { get; set; }
        public string tu_e { get; set; }
        public string tu_lb_s { get; set; }
        public string tu_lb_e { get; set; }
        public int we { get; set; }
        public string we_s { get; set; }
        public string we_e { get; set; }
        public string we_lb_s { get; set; }
        public string we_lb_e { get; set; }
        public int th { get; set; }
        public string th_s { get; set; }
        public string th_e { get; set; }
        public string th_lb_s { get; set; }
        public string th_lb_e { get; set; }
        public int fr { get; set; }
        public string fr_s { get; set; }
        public string fr_e { get; set; }
        public string fr_lb_s { get; set; }
        public string fr_lb_e { get; set; }
        public int sa { get; set; }
        public string sa_s { get; set; }
        public string sa_e { get; set; }
        public string sa_lb_s { get; set; }
        public string sa_lb_e { get; set; }
        public int su { get; set; }
        public string su_s { get; set; }
        public string su_e { get; set; }
        public string su_lb_s { get; set; }
        public string su_lb_e { get; set; }
    }


}
