using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class filter_tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsStatusFilterEnabled",
                table: "FixedFilterSettings",
                newName: "ShowStatus");

            migrationBuilder.RenameColumn(
                name: "IsReadyToShipFilterEnabled",
                table: "FixedFilterSettings",
                newName: "ShowShip");

            migrationBuilder.RenameColumn(
                name: "IsPriceFilterEnabled",
                table: "FixedFilterSettings",
                newName: "ShowPrice");

            migrationBuilder.RenameColumn(
                name: "IsLoyalityFilterEnabled",
                table: "FixedFilterSettings",
                newName: "ShowLoyality");

            migrationBuilder.RenameColumn(
                name: "IsBrandFilterEnabled",
                table: "FixedFilterSettings",
                newName: "ShowBrands");

            migrationBuilder.RenameColumn(
                name: "IsActionsFilterEnabled",
                table: "FixedFilterSettings",
                newName: "ShowActions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShowStatus",
                table: "FixedFilterSettings",
                newName: "IsStatusFilterEnabled");

            migrationBuilder.RenameColumn(
                name: "ShowShip",
                table: "FixedFilterSettings",
                newName: "IsReadyToShipFilterEnabled");

            migrationBuilder.RenameColumn(
                name: "ShowPrice",
                table: "FixedFilterSettings",
                newName: "IsPriceFilterEnabled");

            migrationBuilder.RenameColumn(
                name: "ShowLoyality",
                table: "FixedFilterSettings",
                newName: "IsLoyalityFilterEnabled");

            migrationBuilder.RenameColumn(
                name: "ShowBrands",
                table: "FixedFilterSettings",
                newName: "IsBrandFilterEnabled");

            migrationBuilder.RenameColumn(
                name: "ShowActions",
                table: "FixedFilterSettings",
                newName: "IsActionsFilterEnabled");
        }
    }
}
