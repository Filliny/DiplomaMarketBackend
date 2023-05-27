using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class customergroups_premissions_relation_again : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerGroups_CustomerGroups_CustomerGroupModelId",
                table: "CustomerGroups");

            migrationBuilder.DropIndex(
                name: "IX_CustomerGroups_CustomerGroupModelId",
                table: "CustomerGroups");

            migrationBuilder.DropColumn(
                name: "CustomerGroupModelId",
                table: "CustomerGroups");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerGroupModelId",
                table: "CustomerGroups",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerGroups_CustomerGroupModelId",
                table: "CustomerGroups",
                column: "CustomerGroupModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerGroups_CustomerGroups_CustomerGroupModelId",
                table: "CustomerGroups",
                column: "CustomerGroupModelId",
                principalTable: "CustomerGroups",
                principalColumn: "Id");
        }
    }
}
