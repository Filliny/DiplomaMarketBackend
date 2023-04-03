using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class art_textcontent_relation : Migration
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

            migrationBuilder.AlterColumn<DateTime>(
                name: "RegDate",
                table: "Users",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                table: "Articles",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

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

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Articles",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

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

            migrationBuilder.AlterColumn<DateTime>(
                name: "RegDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                table: "Articles",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

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

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Articles",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

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
    }
}
