using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class art_translation_relation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_translations_textContents_TextContentId",
                table: "translations");

            migrationBuilder.AlterColumn<int>(
                name: "TextContentId",
                table: "translations",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_translations_textContents_TextContentId",
                table: "translations",
                column: "TextContentId",
                principalTable: "textContents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_translations_textContents_TextContentId",
                table: "translations");

            migrationBuilder.AlterColumn<int>(
                name: "TextContentId",
                table: "translations",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_translations_textContents_TextContentId",
                table: "translations",
                column: "TextContentId",
                principalTable: "textContents",
                principalColumn: "Id");
        }
    }
}
