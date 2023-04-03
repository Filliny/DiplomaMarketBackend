using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class charactesistic_groups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CharacteristicGroupId",
                table: "ArticleCharacteristics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "ArticleCharacteristics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "ArticleCharacteristics",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CharacteristicGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    group_order = table.Column<int>(type: "integer", nullable: false),
                    groupTitleId = table.Column<int>(type: "integer", nullable: true),
                    rztk_grp_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacteristicGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacteristicGroups_textContents_groupTitleId",
                        column: x => x.groupTitleId,
                        principalTable: "textContents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleCharacteristics_CharacteristicGroupId",
                table: "ArticleCharacteristics",
                column: "CharacteristicGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacteristicGroups_groupTitleId",
                table: "CharacteristicGroups",
                column: "groupTitleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleCharacteristics_CharacteristicGroups_CharacteristicG~",
                table: "ArticleCharacteristics",
                column: "CharacteristicGroupId",
                principalTable: "CharacteristicGroups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleCharacteristics_CharacteristicGroups_CharacteristicG~",
                table: "ArticleCharacteristics");

            migrationBuilder.DropTable(
                name: "CharacteristicGroups");

            migrationBuilder.DropIndex(
                name: "IX_ArticleCharacteristics_CharacteristicGroupId",
                table: "ArticleCharacteristics");

            migrationBuilder.DropColumn(
                name: "CharacteristicGroupId",
                table: "ArticleCharacteristics");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "ArticleCharacteristics");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "ArticleCharacteristics");
        }
    }
}
