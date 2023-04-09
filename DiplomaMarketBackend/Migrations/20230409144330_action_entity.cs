using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class action_entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActionId",
                table: "Articles",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ActionModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NameId = table.Column<int>(type: "integer", nullable: true),
                    DescriptionId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActionModel_textContents_DescriptionId",
                        column: x => x.DescriptionId,
                        principalTable: "textContents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActionModel_textContents_NameId",
                        column: x => x.NameId,
                        principalTable: "textContents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_ActionId",
                table: "Articles",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionModel_DescriptionId",
                table: "ActionModel",
                column: "DescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionModel_NameId",
                table: "ActionModel",
                column: "NameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_ActionModel_ActionId",
                table: "Articles",
                column: "ActionId",
                principalTable: "ActionModel",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_ActionModel_ActionId",
                table: "Articles");

            migrationBuilder.DropTable(
                name: "ActionModel");

            migrationBuilder.DropIndex(
                name: "IX_Articles_ActionId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "ActionId",
                table: "Articles");
        }
    }
}
