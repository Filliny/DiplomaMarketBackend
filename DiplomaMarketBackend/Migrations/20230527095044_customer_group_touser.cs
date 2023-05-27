using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class customer_group_touser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerGroupModelId",
                table: "CustomerGroups",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerGroupId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerGroups_CustomerGroupModelId",
                table: "CustomerGroups",
                column: "CustomerGroupModelId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CustomerGroupId",
                table: "AspNetUsers",
                column: "CustomerGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_CustomerGroups_CustomerGroupId",
                table: "AspNetUsers",
                column: "CustomerGroupId",
                principalTable: "CustomerGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerGroups_CustomerGroups_CustomerGroupModelId",
                table: "CustomerGroups",
                column: "CustomerGroupModelId",
                principalTable: "CustomerGroups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_CustomerGroups_CustomerGroupId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerGroups_CustomerGroups_CustomerGroupModelId",
                table: "CustomerGroups");

            migrationBuilder.DropIndex(
                name: "IX_CustomerGroups_CustomerGroupModelId",
                table: "CustomerGroups");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CustomerGroupId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CustomerGroupModelId",
                table: "CustomerGroups");

            migrationBuilder.DropColumn(
                name: "CustomerGroupId",
                table: "AspNetUsers");
        }
    }
}
