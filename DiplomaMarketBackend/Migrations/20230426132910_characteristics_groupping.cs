using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class characteristics_groupping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CharacteristicValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TitleId = table.Column<int>(type: "integer", nullable: true),
                    href = table.Column<string>(type: "text", nullable: true),
                    CharacteristicTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacteristicValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacteristicValues_ArticleCharacteristics_CharacteristicT~",
                        column: x => x.CharacteristicTypeId,
                        principalTable: "ArticleCharacteristics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacteristicValues_textContents_TitleId",
                        column: x => x.TitleId,
                        principalTable: "textContents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ArticleModelValueModel",
                columns: table => new
                {
                    ArticlesId = table.Column<int>(type: "integer", nullable: false),
                    CharacteristicValuesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleModelValueModel", x => new { x.ArticlesId, x.CharacteristicValuesId });
                    table.ForeignKey(
                        name: "FK_ArticleModelValueModel_Articles_ArticlesId",
                        column: x => x.ArticlesId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleModelValueModel_CharacteristicValues_CharacteristicV~",
                        column: x => x.CharacteristicValuesId,
                        principalTable: "CharacteristicValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleModelValueModel_CharacteristicValuesId",
                table: "ArticleModelValueModel",
                column: "CharacteristicValuesId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacteristicValues_CharacteristicTypeId",
                table: "CharacteristicValues",
                column: "CharacteristicTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacteristicValues_TitleId",
                table: "CharacteristicValues",
                column: "TitleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleModelValueModel");

            migrationBuilder.DropTable(
                name: "CharacteristicValues");
        }
    }
}
