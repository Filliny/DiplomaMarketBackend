using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class videotype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Videos_VideoTypes_TypeId",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_Videos_TypeId",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Videos");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Videos",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Videos");

            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "Videos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Videos_TypeId",
                table: "Videos",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_VideoTypes_TypeId",
                table: "Videos",
                column: "TypeId",
                principalTable: "VideoTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
