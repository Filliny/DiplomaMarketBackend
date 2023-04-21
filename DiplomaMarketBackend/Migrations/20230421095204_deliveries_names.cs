using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class deliveries_names : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Deliveries");

            migrationBuilder.AddColumn<int>(
                name: "NameId",
                table: "Deliveries",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_NameId",
                table: "Deliveries",
                column: "NameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_textContents_NameId",
                table: "Deliveries",
                column: "NameId",
                principalTable: "textContents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_textContents_NameId",
                table: "Deliveries");

            migrationBuilder.DropIndex(
                name: "IX_Deliveries_NameId",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "NameId",
                table: "Deliveries");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Deliveries",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
