using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class remove_old_charakteristics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Values");

            migrationBuilder.CreateTable(
                name: "CharacteristicValueModel_del",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TitleId = table.Column<int>(type: "integer", nullable: true),
                    href = table.Column<string>(type: "text", nullable: true),
                    CharacteristicTypeId = table.Column<int>(type: "integer", nullable: false),
                    articleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacteristicValueModel_del", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacteristicValueModel_del_ArticleCharacteristics_Charact~",
                        column: x => x.CharacteristicTypeId,
                        principalTable: "ArticleCharacteristics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacteristicValueModel_del_Articles_articleId",
                        column: x => x.articleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacteristicValueModel_del_textContents_TitleId",
                        column: x => x.TitleId,
                        principalTable: "textContents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacteristicValueModel_del_articleId",
                table: "CharacteristicValueModel_del",
                column: "articleId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacteristicValueModel_del_CharacteristicTypeId",
                table: "CharacteristicValueModel_del",
                column: "CharacteristicTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacteristicValueModel_del_TitleId",
                table: "CharacteristicValueModel_del",
                column: "TitleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacteristicValueModel_del");

            migrationBuilder.CreateTable(
                name: "Values",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    articleId = table.Column<int>(type: "integer", nullable: false),
                    CharacteristicTypeId = table.Column<int>(type: "integer", nullable: false),
                    TitleId = table.Column<int>(type: "integer", nullable: true),
                    href = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Values", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Values_ArticleCharacteristics_CharacteristicTypeId",
                        column: x => x.CharacteristicTypeId,
                        principalTable: "ArticleCharacteristics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Values_Articles_articleId",
                        column: x => x.articleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Values_textContents_TitleId",
                        column: x => x.TitleId,
                        principalTable: "textContents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Values_articleId",
                table: "Values",
                column: "articleId");

            migrationBuilder.CreateIndex(
                name: "IX_Values_CharacteristicTypeId",
                table: "Values",
                column: "CharacteristicTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Values_TitleId",
                table: "Values",
                column: "TitleId");
        }
    }
}
