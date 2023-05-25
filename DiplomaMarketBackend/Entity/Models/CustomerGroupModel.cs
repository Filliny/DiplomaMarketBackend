namespace DiplomaMarketBackend.Entity.Models
{
    public class CustomerGroupModel
    {
        public int Id { get; set; }
        public string Name { get; set; }  = string.Empty;
        public List<PermissionModel> Permissions { get; set; }

        public CustomerGroupModel() { 
        
            Permissions = new List<PermissionModel>();  
        }
    }
}
