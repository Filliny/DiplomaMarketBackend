using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class charakteristic_group_fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleCharacteristics_CharacteristicGroups_CharacteristicG~",
                table: "ArticleCharacteristics");

            migrationBuilder.DropIndex(
                name: "IX_ArticleCharacteristics_CharacteristicGroupId",
                table: "ArticleCharacteristics");

            migrationBuilder.DropColumn(
                name: "CharacteristicGroupId",
                table: "ArticleCharacteristics");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleCharacteristics_GroupId",
                table: "ArticleCharacteristics",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleCharacteristics_CharacteristicGroups_GroupId",
                table: "ArticleCharacteristics",
                column: "GroupId",
                principalTable: "CharacteristicGroups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleCharacteristics_CharacteristicGroups_GroupId",
                table: "ArticleCharacteristics");

            migrationBuilder.DropIndex(
                name: "IX_ArticleCharacteristics_GroupId",
                table: "ArticleCharacteristics");

            migrationBuilder.AddColumn<int>(
                name: "CharacteristicGroupId",
                table: "ArticleCharacteristics",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArticleCharacteristics_CharacteristicGroupId",
                table: "ArticleCharacteristics",
                column: "CharacteristicGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleCharacteristics_CharacteristicGroups_CharacteristicG~",
                table: "ArticleCharacteristics",
                column: "CharacteristicGroupId",
                principalTable: "CharacteristicGroups",
                principalColumn: "Id");
        }
    }
}
