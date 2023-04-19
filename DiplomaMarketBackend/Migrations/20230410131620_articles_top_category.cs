using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class articles_top_category : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TopCategoryId",
                table: "Articles",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_TopCategoryId",
                table: "Articles",
                column: "TopCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Categories_TopCategoryId",
                table: "Articles",
                column: "TopCategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Categories_TopCategoryId",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_TopCategoryId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "TopCategoryId",
                table: "Articles");
        }
    }
}
