using Lessons3.Entity.Models;

namespace DiplomaMarketBackend.Entity.Models
{
    public class CustomerGroupModel
    {
        public int Id { get; set; }
        public string Name { get; set; }  = string.Empty;
        public List<PermissionKeysModel> PermissionsKeys { get; set; }
        public List<UserModel> Customers { get; set; }

        public CustomerGroupModel() { 
        
            PermissionsKeys = new List<PermissionKeysModel>();  
            Customers = new List<UserModel>();
        }
    }
}
