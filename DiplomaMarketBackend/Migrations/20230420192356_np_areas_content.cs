using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class np_areas_content : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NameId",
                table: "Areas",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Areas_NameId",
                table: "Areas",
                column: "NameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Areas_textContents_NameId",
                table: "Areas",
                column: "NameId",
                principalTable: "textContents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Areas_textContents_NameId",
                table: "Areas");

            migrationBuilder.DropIndex(
                name: "IX_Areas_NameId",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "NameId",
                table: "Areas");
        }
    }
}
