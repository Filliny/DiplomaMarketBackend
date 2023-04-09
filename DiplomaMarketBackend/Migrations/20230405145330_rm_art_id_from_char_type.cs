using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class rm_art_id_from_char_type : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleCharacteristics_Articles_ArticleId",
                table: "ArticleCharacteristics");

            migrationBuilder.DropIndex(
                name: "IX_ArticleCharacteristics_ArticleId",
                table: "ArticleCharacteristics");

            migrationBuilder.DropColumn(
                name: "ArticleId",
                table: "ArticleCharacteristics");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArticleId",
                table: "ArticleCharacteristics",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArticleCharacteristics_ArticleId",
                table: "ArticleCharacteristics",
                column: "ArticleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleCharacteristics_Articles_ArticleId",
                table: "ArticleCharacteristics",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id");
        }
    }
}
