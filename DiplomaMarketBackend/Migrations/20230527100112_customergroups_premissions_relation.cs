using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class customergroups_premissions_relation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_CustomerGroups_CustomerGroupModelId",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_CustomerGroupModelId",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "CustomerGroupModelId",
                table: "Permissions");

            migrationBuilder.CreateTable(
                name: "CustomerGroupModelPermissionModel",
                columns: table => new
                {
                    CustomerGroupsId = table.Column<int>(type: "integer", nullable: false),
                    PermissionsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerGroupModelPermissionModel", x => new { x.CustomerGroupsId, x.PermissionsId });
                    table.ForeignKey(
                        name: "FK_CustomerGroupModelPermissionModel_CustomerGroups_CustomerGr~",
                        column: x => x.CustomerGroupsId,
                        principalTable: "CustomerGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerGroupModelPermissionModel_Permissions_PermissionsId",
                        column: x => x.PermissionsId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerGroupModelPermissionModel_PermissionsId",
                table: "CustomerGroupModelPermissionModel",
                column: "PermissionsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerGroupModelPermissionModel");

            migrationBuilder.AddColumn<int>(
                name: "CustomerGroupModelId",
                table: "Permissions",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_CustomerGroupModelId",
                table: "Permissions",
                column: "CustomerGroupModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_CustomerGroups_CustomerGroupModelId",
                table: "Permissions",
                column: "CustomerGroupModelId",
                principalTable: "CustomerGroups",
                principalColumn: "Id");
        }
    }
}
