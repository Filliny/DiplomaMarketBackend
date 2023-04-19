using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class review_model_naming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Reviewtype",
                table: "Reviews",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "ReviewName",
                table: "Reviews",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ReviewEmail",
                table: "Reviews",
                newName: "Email");

            migrationBuilder.AddColumn<bool>(
                name: "ReviewApproved",
                table: "Reviews",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewApproved",
                table: "Reviews");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Reviews",
                newName: "Reviewtype");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Reviews",
                newName: "ReviewName");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Reviews",
                newName: "ReviewEmail");
        }
    }
}
