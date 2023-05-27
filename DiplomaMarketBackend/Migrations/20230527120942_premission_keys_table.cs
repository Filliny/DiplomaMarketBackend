using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class premission_keys_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerGroupModelPermissionModel");

            migrationBuilder.CreateTable(
                name: "PermissionKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Allowed = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    PermissionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PermissionKeys_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerGroupModelPermissionKeysModel",
                columns: table => new
                {
                    CustomerGroupsId = table.Column<int>(type: "integer", nullable: false),
                    PermissionsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerGroupModelPermissionKeysModel", x => new { x.CustomerGroupsId, x.PermissionsId });
                    table.ForeignKey(
                        name: "FK_CustomerGroupModelPermissionKeysModel_CustomerGroups_Custom~",
                        column: x => x.CustomerGroupsId,
                        principalTable: "CustomerGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerGroupModelPermissionKeysModel_PermissionKeys_Permis~",
                        column: x => x.PermissionsId,
                        principalTable: "PermissionKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerGroupModelPermissionKeysModel_PermissionsId",
                table: "CustomerGroupModelPermissionKeysModel",
                column: "PermissionsId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionKeys_Allowed_PermissionId",
                table: "PermissionKeys",
                columns: new[] { "Allowed", "PermissionId" });

            migrationBuilder.CreateIndex(
                name: "IX_PermissionKeys_PermissionId",
                table: "PermissionKeys",
                column: "PermissionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerGroupModelPermissionKeysModel");

            migrationBuilder.DropTable(
                name: "PermissionKeys");

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
    }
}
