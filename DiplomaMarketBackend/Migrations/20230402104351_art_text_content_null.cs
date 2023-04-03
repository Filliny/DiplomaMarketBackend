using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class art_text_content_null : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_textContents_DescriptionId",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK_Articles_textContents_DocketId",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK_Articles_textContents_TitleId",
                table: "Articles");

            migrationBuilder.AlterColumn<int>(
                name: "TitleId",
                table: "Articles",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "DocketId",
                table: "Articles",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "DescriptionId",
                table: "Articles",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_textContents_DescriptionId",
                table: "Articles",
                column: "DescriptionId",
                principalTable: "textContents",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_textContents_DocketId",
                table: "Articles",
                column: "DocketId",
                principalTable: "textContents",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_textContents_TitleId",
                table: "Articles",
                column: "TitleId",
                principalTable: "textContents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_textContents_DescriptionId",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK_Articles_textContents_DocketId",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK_Articles_textContents_TitleId",
                table: "Articles");

            migrationBuilder.AlterColumn<int>(
                name: "TitleId",
                table: "Articles",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DocketId",
                table: "Articles",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DescriptionId",
                table: "Articles",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_textContents_DescriptionId",
                table: "Articles",
                column: "DescriptionId",
                principalTable: "textContents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_textContents_DocketId",
                table: "Articles",
                column: "DocketId",
                principalTable: "textContents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_textContents_TitleId",
                table: "Articles",
                column: "TitleId",
                principalTable: "textContents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
