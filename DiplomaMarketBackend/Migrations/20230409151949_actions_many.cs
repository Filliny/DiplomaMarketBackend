using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class actions_many : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_ActionModel_ActionId",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_ActionId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "ActionId",
                table: "Articles");

            migrationBuilder.CreateTable(
                name: "ActionModelArticleModel",
                columns: table => new
                {
                    ActionsId = table.Column<int>(type: "integer", nullable: false),
                    ArticlesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionModelArticleModel", x => new { x.ActionsId, x.ArticlesId });
                    table.ForeignKey(
                        name: "FK_ActionModelArticleModel_ActionModel_ActionsId",
                        column: x => x.ActionsId,
                        principalTable: "ActionModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActionModelArticleModel_Articles_ArticlesId",
                        column: x => x.ArticlesId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActionModelArticleModel_ArticlesId",
                table: "ActionModelArticleModel",
                column: "ArticlesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionModelArticleModel");

            migrationBuilder.AddColumn<int>(
                name: "ActionId",
                table: "Articles",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_ActionId",
                table: "Articles",
                column: "ActionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_ActionModel_ActionId",
                table: "Articles",
                column: "ActionId",
                principalTable: "ActionModel",
                principalColumn: "Id");
        }
    }
}
