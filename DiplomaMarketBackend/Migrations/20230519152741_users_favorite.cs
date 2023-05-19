using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class users_favorite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArticleModelUserModel",
                columns: table => new
                {
                    FavoriteUsersId = table.Column<string>(type: "text", nullable: false),
                    FavoritesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleModelUserModel", x => new { x.FavoriteUsersId, x.FavoritesId });
                    table.ForeignKey(
                        name: "FK_ArticleModelUserModel_Articles_FavoritesId",
                        column: x => x.FavoritesId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleModelUserModel_AspNetUsers_FavoriteUsersId",
                        column: x => x.FavoriteUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleModelUserModel_FavoritesId",
                table: "ArticleModelUserModel",
                column: "FavoritesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleModelUserModel");
        }
    }
}
