namespace InventoryBackend
{
    public static class SessionManager
    {
        public static string Username { get; set; } = "";
        public static string FullName { get; set; } = "";
        public static string Email { get; set; } = "";
        public static string Role { get; set; } = "";
        public static string Permissions { get; set; } = "";

        public static bool IsSystemAdmin()
        {
            return Role == "System Admin";
        }

        public static bool IsAdmin()
        {
            return Role == "Admin";
        }

        public static bool IsStaff()
        {
            return Role == "Staff";
        }

        public static bool HasPermission(string permissionName)
        {
            if (IsSystemAdmin())
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(Permissions))
            {
                return false;
            }

            return Permissions.ToLower().Contains(permissionName.ToLower());
        }

        public static bool CanManageUsers()
        {
            return IsSystemAdmin() || HasPermission("Manage Users");
        }

        public static bool CanAccessSuppliers()
        {
            return IsSystemAdmin() || HasPermission("Manage Suppliers");
        }

        public static bool CanManageSuppliers()
        {
            return IsSystemAdmin() || HasPermission("Manage Suppliers");
        }

        public static bool CanDeleteItems()
        {
            return IsSystemAdmin() || HasPermission("Delete Inventory Items");
        }

        public static bool CanViewReports()
        {
            return IsSystemAdmin() || IsAdmin();
        }

        public static bool CanDeleteTransactions()
        {
            return IsSystemAdmin();
        }
    }
}
