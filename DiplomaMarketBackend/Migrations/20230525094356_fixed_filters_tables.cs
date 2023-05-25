using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class fixed_filters_tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FixedFilterLocalization",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BrandFilterNameId = table.Column<int>(type: "integer", nullable: true),
                    PriceFilterNameId = table.Column<int>(type: "integer", nullable: true),
                    ReadyToShipFilterNameId = table.Column<int>(type: "integer", nullable: true),
                    ReadyToShipValueNameId = table.Column<int>(type: "integer", nullable: true),
                    ActionsFilterNameId = table.Column<int>(type: "integer", nullable: true),
                    ActionValueNameId = table.Column<int>(type: "integer", nullable: true),
                    LoyalityFilterNameId = table.Column<int>(type: "integer", nullable: true),
                    WithBonusesValueNameId = table.Column<int>(type: "integer", nullable: true),
                    ArticleStatusFilterNameId = table.Column<int>(type: "integer", nullable: true),
                    InStockValueNameId = table.Column<int>(type: "integer", nullable: true),
                    OutOfStockValueNameId = table.Column<int>(type: "integer", nullable: true),
                    ItemEndedValueNameId = table.Column<int>(type: "integer", nullable: true),
                    EndingSoonValueNameId = table.Column<int>(type: "integer", nullable: true),
                    ItemAwaitedValueNameId = table.Column<int>(type: "integer", nullable: true),
                    LocalesAvailable = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixedFilterLocalization", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FixedFilterLocalization_textContents_ActionValueNameId",
                        column: x => x.ActionValueNameId,
                        principalTable: "textContents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FixedFilterLocalization_textContents_ActionsFilterNameId",
                        column: x => x.ActionsFilterNameId,
                        principalTable: "textContents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FixedFilterLocalization_textContents_ArticleStatusFilterNam~",
                        column: x => x.ArticleStatusFilterNameId,
                        principalTable: "textContents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FixedFilterLocalization_textContents_BrandFilterNameId",
                        column: x => x.BrandFilterNameId,
                        principalTable: "textContents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FixedFilterLocalization_textContents_EndingSoonValueNameId",
                        column: x => x.EndingSoonValueNameId,
                        principalTable: "textContents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FixedFilterLocalization_textContents_InStockValueNameId",
                        column: x => x.InStockValueNameId,
                        principalTable: "textContents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FixedFilterLocalization_textContents_ItemAwaitedValueNameId",
                        column: x => x.ItemAwaitedValueNameId,
                        principalTable: "textContents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FixedFilterLocalization_textContents_ItemEndedValueNameId",
                        column: x => x.ItemEndedValueNameId,
                        principalTable: "textContents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FixedFilterLocalization_textContents_LoyalityFilterNameId",
                        column: x => x.LoyalityFilterNameId,
                        principalTable: "textContents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FixedFilterLocalization_textContents_OutOfStockValueNameId",
                        column: x => x.OutOfStockValueNameId,
                        principalTable: "textContents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FixedFilterLocalization_textContents_PriceFilterNameId",
                        column: x => x.PriceFilterNameId,
                        principalTable: "textContents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FixedFilterLocalization_textContents_ReadyToShipFilterNameId",
                        column: x => x.ReadyToShipFilterNameId,
                        principalTable: "textContents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FixedFilterLocalization_textContents_ReadyToShipValueNameId",
                        column: x => x.ReadyToShipValueNameId,
                        principalTable: "textContents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FixedFilterLocalization_textContents_WithBonusesValueNameId",
                        column: x => x.WithBonusesValueNameId,
                        principalTable: "textContents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FixedFilterSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    IsBrandFilerEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsPriceFilerEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsReadyToShipFilterEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsActionsFilterEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsLoyalityFilterEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsStatusFilterEnabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixedFilterSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FixedFilterSettings_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FixedFilterLocalization_ActionsFilterNameId",
                table: "FixedFilterLocalization",
                column: "ActionsFilterNameId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedFilterLocalization_ActionValueNameId",
                table: "FixedFilterLocalization",
                column: "ActionValueNameId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedFilterLocalization_ArticleStatusFilterNameId",
                table: "FixedFilterLocalization",
                column: "ArticleStatusFilterNameId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedFilterLocalization_BrandFilterNameId",
                table: "FixedFilterLocalization",
                column: "BrandFilterNameId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedFilterLocalization_EndingSoonValueNameId",
                table: "FixedFilterLocalization",
                column: "EndingSoonValueNameId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedFilterLocalization_InStockValueNameId",
                table: "FixedFilterLocalization",
                column: "InStockValueNameId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedFilterLocalization_ItemAwaitedValueNameId",
                table: "FixedFilterLocalization",
                column: "ItemAwaitedValueNameId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedFilterLocalization_ItemEndedValueNameId",
                table: "FixedFilterLocalization",
                column: "ItemEndedValueNameId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedFilterLocalization_LoyalityFilterNameId",
                table: "FixedFilterLocalization",
                column: "LoyalityFilterNameId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedFilterLocalization_OutOfStockValueNameId",
                table: "FixedFilterLocalization",
                column: "OutOfStockValueNameId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedFilterLocalization_PriceFilterNameId",
                table: "FixedFilterLocalization",
                column: "PriceFilterNameId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedFilterLocalization_ReadyToShipFilterNameId",
                table: "FixedFilterLocalization",
                column: "ReadyToShipFilterNameId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedFilterLocalization_ReadyToShipValueNameId",
                table: "FixedFilterLocalization",
                column: "ReadyToShipValueNameId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedFilterLocalization_WithBonusesValueNameId",
                table: "FixedFilterLocalization",
                column: "WithBonusesValueNameId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedFilterSettings_CategoryId",
                table: "FixedFilterSettings",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FixedFilterLocalization");

            migrationBuilder.DropTable(
                name: "FixedFilterSettings");
        }
    }
}
