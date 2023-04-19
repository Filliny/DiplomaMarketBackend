using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class category_extension : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShowInCategoryId",
                table: "Categories",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "Categories",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowInCategoryId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "Categories");
        }
    }
}
