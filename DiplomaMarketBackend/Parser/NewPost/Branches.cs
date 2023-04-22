namespace DiplomaMarketBackend.Parser.NewPost.Branches
{
  
    public class Datum
    {
        public string SiteKey { get; set; }
        public string Description { get; set; }
        public string DescriptionRu { get; set; }
        public string ShortAddress { get; set; }
        public string ShortAddressRu { get; set; }
        public string Phone { get; set; }
        public string TypeOfWarehouse { get; set; }
        public string Ref { get; set; }
        public string Number { get; set; }
        public string CityRef { get; set; }
        public string CityDescription { get; set; }
        public string CityDescriptionRu { get; set; }
        public string SettlementRef { get; set; }
        public string SettlementDescription { get; set; }
        public string SettlementAreaDescription { get; set; }
        public string SettlementRegionsDescription { get; set; }
        public string SettlementTypeDescription { get; set; }
        public string SettlementTypeDescriptionRu { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string PostFinance { get; set; }
        public string BicycleParking { get; set; }
        public string PaymentAccess { get; set; }
        public string POSTerminal { get; set; }
        public string InternationalShipping { get; set; }
        public string SelfServiceWorkplacesCount { get; set; }
        public string TotalMaxWeightAllowed { get; set; }
        public string PlaceMaxWeightAllowed { get; set; }
        public SendingLimitationsOnDimensions SendingLimitationsOnDimensions { get; set; }
        public ReceivingLimitationsOnDimensions ReceivingLimitationsOnDimensions { get; set; }
        public Reception Reception { get; set; }
        public Delivery Delivery { get; set; }
        public Schedule Schedule { get; set; }
        public string DistrictCode { get; set; }
        public string WarehouseStatus { get; set; }
        public string WarehouseStatusDate { get; set; }
        public string CategoryOfWarehouse { get; set; }
        public string Direct { get; set; }
        public string RegionCity { get; set; }
        public string WarehouseForAgent { get; set; }
        public string GeneratorEnabled { get; set; }
        public string MaxDeclaredCost { get; set; }
        public string WorkInMobileAwis { get; set; }
        public string DenyToSelect { get; set; }
        public string CanGetMoneyTransfer { get; set; }
        public string OnlyReceivingParcel { get; set; }
        public string PostMachineType { get; set; }
        public string PostalCodeUA { get; set; }
        public string WarehouseIndex { get; set; }
    }

    public class Delivery
    {
        public string Monday { get; set; }
        public string Tuesday { get; set; }
        public string Wednesday { get; set; }
        public string Thursday { get; set; }
        public string Friday { get; set; }
        public string Saturday { get; set; }
        public string Sunday { get; set; }
    }

    public class Info
    {
        public int totalCount { get; set; }
    }

    public class ReceivingLimitationsOnDimensions
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Length { get; set; }
    }

    public class Reception
    {
        public string Monday { get; set; }
        public string Tuesday { get; set; }
        public string Wednesday { get; set; }
        public string Thursday { get; set; }
        public string Friday { get; set; }
        public string Saturday { get; set; }
        public string Sunday { get; set; }
    }

    public class Root
    {
        public bool success { get; set; }
        public List<Datum> data { get; set; }
        public List<object> errors { get; set; }
        public List<object> warnings { get; set; }
        public Info info { get; set; }
        public List<object> messageCodes { get; set; }
        public List<object> errorCodes { get; set; }
        public List<object> warningCodes { get; set; }
        public List<object> infoCodes { get; set; }
    }

    public class Schedule
    {
        public string Monday { get; set; }
        public string Tuesday { get; set; }
        public string Wednesday { get; set; }
        public string Thursday { get; set; }
        public string Friday { get; set; }
        public string Saturday { get; set; }
        public string Sunday { get; set; }
    }

    public class SendingLimitationsOnDimensions
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Length { get; set; }
    }


}
