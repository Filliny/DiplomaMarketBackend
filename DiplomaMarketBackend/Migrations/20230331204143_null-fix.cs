using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class nullfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_textContents_DescriptionId",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK_Breadcrumbs_textContents_TitleId",
                table: "Breadcrumbs");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_textContents_DescriptionId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Tags");

            migrationBuilder.AddColumn<int>(
                name: "NameId",
                table: "Tags",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TitleId",
                table: "Tags",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DescriptionId",
                table: "Categories",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "TitleId",
                table: "Breadcrumbs",
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

            migrationBuilder.CreateIndex(
                name: "IX_Tags_NameId",
                table: "Tags",
                column: "NameId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TitleId",
                table: "Tags",
                column: "TitleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_textContents_DescriptionId",
                table: "Articles",
                column: "DescriptionId",
                principalTable: "textContents",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Breadcrumbs_textContents_TitleId",
                table: "Breadcrumbs",
                column: "TitleId",
                principalTable: "textContents",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_textContents_DescriptionId",
                table: "Categories",
                column: "DescriptionId",
                principalTable: "textContents",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_textContents_NameId",
                table: "Tags",
                column: "NameId",
                principalTable: "textContents",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_textContents_TitleId",
                table: "Tags",
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
                name: "FK_Breadcrumbs_textContents_TitleId",
                table: "Breadcrumbs");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_textContents_DescriptionId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_textContents_NameId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_textContents_TitleId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_NameId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_TitleId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "NameId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "TitleId",
                table: "Tags");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Tags",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Tags",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DescriptionId",
                table: "Categories",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TitleId",
                table: "Breadcrumbs",
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
                name: "FK_Breadcrumbs_textContents_TitleId",
                table: "Breadcrumbs",
                column: "TitleId",
                principalTable: "textContents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_textContents_DescriptionId",
                table: "Categories",
                column: "DescriptionId",
                principalTable: "textContents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
