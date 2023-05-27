namespace DiplomaMarketBackend.Models
{

    /// <summary>
    /// Entity for customers Role Creation\Edition (for /api/Roles/create)
    /// </summary>
    public class CustomersRole
    {
        public class SelectedPermission
        {

            /// <summary>
            /// Permission id
            /// </summary>
            public int id { get; set; }

            /// <summary>
            /// Permission Name
            /// </summary>
            public string? name { get; set; } = string.Empty;

            /// <summary>
            /// Is reading Allowed
            /// </summary>
            public bool read_allowed { get; set; } = false;

            /// <summary>
            /// Is write Allowed
            /// </summary>
            public bool write_allowed { get; set; } = false;
        }


        /// <summary>
        /// Group id - must be 0 on creation or exist if for update
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Group name for creation
        /// </summary>
        public string name { get; set; } = string.Empty;

        /// <summary>
        /// List of permissions for this group
        /// </summary>
        public List<SelectedPermission> permissions { get; set; } = new List<SelectedPermission>();

    }
}
