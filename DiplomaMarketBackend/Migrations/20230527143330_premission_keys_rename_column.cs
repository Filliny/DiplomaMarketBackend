using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class premission_keys_rename_column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PermissionsId",
                table: "CustomerGroupModelPermissionKeysModel",
                newName: "PermissionsKeysId");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerGroupModelPermissionKeysModel_PermissionsId",
                table: "CustomerGroupModelPermissionKeysModel",
                newName: "IX_CustomerGroupModelPermissionKeysModel_PermissionsKeysId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PermissionsKeysId",
                table: "CustomerGroupModelPermissionKeysModel",
                newName: "PermissionsId");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerGroupModelPermissionKeysModel_PermissionsKeysId",
                table: "CustomerGroupModelPermissionKeysModel",
                newName: "IX_CustomerGroupModelPermissionKeysModel_PermissionsId");
        }
    }
}
