using Microsoft.EntityFrameworkCore;

namespace DiplomaMarketBackend.Entity.Models
{
    public enum AllowedKey
    {
        read,
        write
    }

    //constraint unique 2 values read and write for one Permission
    [Index(nameof(Allowed),nameof(PermissionId))]

    //Entitiy to define two or more state for one permission name (allow read,write or more)
    public class PermissionKeysModel
    {
        public int Id { get; set; }
        public AllowedKey Allowed { get; set; }
        public string RoleId { get; set; } = string.Empty;
        public int PermissionId { get; set; }
        public PermissionModel? Permission { get; set; }
        public List<CustomerGroupModel> CustomerGroups { get; set; } = new List<CustomerGroupModel>();
    }
}
