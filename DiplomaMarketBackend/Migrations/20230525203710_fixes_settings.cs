using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class fixes_settings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsPriceFilerEnabled",
                table: "FixedFilterSettings",
                newName: "IsPriceFilterEnabled");

            migrationBuilder.RenameColumn(
                name: "IsBrandFilerEnabled",
                table: "FixedFilterSettings",
                newName: "IsBrandFilterEnabled");

            migrationBuilder.AddColumn<int>(
                name: "PriceStep",
                table: "FixedFilterSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceStep",
                table: "FixedFilterSettings");

            migrationBuilder.RenameColumn(
                name: "IsPriceFilterEnabled",
                table: "FixedFilterSettings",
                newName: "IsPriceFilerEnabled");

            migrationBuilder.RenameColumn(
                name: "IsBrandFilterEnabled",
                table: "FixedFilterSettings",
                newName: "IsBrandFilerEnabled");
        }
    }
}
