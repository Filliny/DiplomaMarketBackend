namespace DiplomaMarketBackend.Entity.Models
{
    //permissions == asp roles name-to-name +  "Read" or "Write" extension
    //fill asp roles from this table on seeding
    public class PermissionModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
