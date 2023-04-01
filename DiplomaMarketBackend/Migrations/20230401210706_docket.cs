using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class docket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DocketId",
                table: "Articles",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_DocketId",
                table: "Articles",
                column: "DocketId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_textContents_DocketId",
                table: "Articles",
                column: "DocketId",
                principalTable: "textContents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_textContents_DocketId",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_DocketId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "DocketId",
                table: "Articles");
        }
    }
}
