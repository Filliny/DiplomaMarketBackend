using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class images_normal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Pictures_base_actionId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Pictures_bigId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Pictures_big_tileId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Pictures_largeId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Pictures_mediumId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Pictures_mobile_largeId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Pictures_mobile_mediumId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Pictures_originalId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Pictures_previewId",
                table: "Images");

            migrationBuilder.AlterColumn<int>(
                name: "width",
                table: "Pictures",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "url",
                table: "Pictures",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "height",
                table: "Pictures",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "previewId",
                table: "Images",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "originalId",
                table: "Images",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "mobile_mediumId",
                table: "Images",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "mobile_largeId",
                table: "Images",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "mediumId",
                table: "Images",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "largeId",
                table: "Images",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "big_tileId",
                table: "Images",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "bigId",
                table: "Images",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "base_actionId",
                table: "Images",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Pictures_base_actionId",
                table: "Images",
                column: "base_actionId",
                principalTable: "Pictures",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Pictures_bigId",
                table: "Images",
                column: "bigId",
                principalTable: "Pictures",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Pictures_big_tileId",
                table: "Images",
                column: "big_tileId",
                principalTable: "Pictures",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Pictures_largeId",
                table: "Images",
                column: "largeId",
                principalTable: "Pictures",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Pictures_mediumId",
                table: "Images",
                column: "mediumId",
                principalTable: "Pictures",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Pictures_mobile_largeId",
                table: "Images",
                column: "mobile_largeId",
                principalTable: "Pictures",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Pictures_mobile_mediumId",
                table: "Images",
                column: "mobile_mediumId",
                principalTable: "Pictures",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Pictures_originalId",
                table: "Images",
                column: "originalId",
                principalTable: "Pictures",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Pictures_previewId",
                table: "Images",
                column: "previewId",
                principalTable: "Pictures",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Pictures_base_actionId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Pictures_bigId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Pictures_big_tileId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Pictures_largeId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Pictures_mediumId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Pictures_mobile_largeId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Pictures_mobile_mediumId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Pictures_originalId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Pictures_previewId",
                table: "Images");

            migrationBuilder.AlterColumn<int>(
                name: "width",
                table: "Pictures",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "url",
                table: "Pictures",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "height",
                table: "Pictures",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "previewId",
                table: "Images",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "originalId",
                table: "Images",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "mobile_mediumId",
                table: "Images",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "mobile_largeId",
                table: "Images",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "mediumId",
                table: "Images",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "largeId",
                table: "Images",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "big_tileId",
                table: "Images",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "bigId",
                table: "Images",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "base_actionId",
                table: "Images",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Pictures_base_actionId",
                table: "Images",
                column: "base_actionId",
                principalTable: "Pictures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Pictures_bigId",
                table: "Images",
                column: "bigId",
                principalTable: "Pictures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Pictures_big_tileId",
                table: "Images",
                column: "big_tileId",
                principalTable: "Pictures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Pictures_largeId",
                table: "Images",
                column: "largeId",
                principalTable: "Pictures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Pictures_mediumId",
                table: "Images",
                column: "mediumId",
                principalTable: "Pictures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Pictures_mobile_largeId",
                table: "Images",
                column: "mobile_largeId",
                principalTable: "Pictures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Pictures_mobile_mediumId",
                table: "Images",
                column: "mobile_mediumId",
                principalTable: "Pictures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Pictures_originalId",
                table: "Images",
                column: "originalId",
                principalTable: "Pictures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Pictures_previewId",
                table: "Images",
                column: "previewId",
                principalTable: "Pictures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
