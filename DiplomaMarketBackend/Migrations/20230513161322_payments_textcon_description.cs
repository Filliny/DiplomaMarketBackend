using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class payments_textcon_description : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DescriptionId",
                table: "PaymentTypes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTypes_DescriptionId",
                table: "PaymentTypes",
                column: "DescriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTypes_textContents_DescriptionId",
                table: "PaymentTypes",
                column: "DescriptionId",
                principalTable: "textContents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTypes_textContents_DescriptionId",
                table: "PaymentTypes");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTypes_DescriptionId",
                table: "PaymentTypes");

            migrationBuilder.DropColumn(
                name: "DescriptionId",
                table: "PaymentTypes");
        }
    }
}
