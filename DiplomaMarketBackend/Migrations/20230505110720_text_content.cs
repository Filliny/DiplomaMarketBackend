using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class text_content : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Warnings_textContents_MessageId",
                table: "Warnings");

            migrationBuilder.DropTable(
                name: "CharacteristicValueModel_del");

            migrationBuilder.AlterColumn<int>(
                name: "MessageId",
                table: "Warnings",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Warnings_textContents_MessageId",
                table: "Warnings",
                column: "MessageId",
                principalTable: "textContents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Warnings_textContents_MessageId",
                table: "Warnings");

            migrationBuilder.AlterColumn<int>(
                name: "MessageId",
                table: "Warnings",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateTable(
                name: "CharacteristicValueModel_del",
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

            migrationBuilder.AddForeignKey(
                name: "FK_Warnings_textContents_MessageId",
                table: "Warnings",
                column: "MessageId",
                principalTable: "textContents",
                principalColumn: "Id");
        }
    }
}
