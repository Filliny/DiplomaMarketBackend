using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class review_model_name : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReviewName",
                table: "Reviews",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewName",
                table: "Reviews");
        }
    }
}
