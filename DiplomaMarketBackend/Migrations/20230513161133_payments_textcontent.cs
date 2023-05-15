using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class payments_textcontent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "PaymentTypes");

            migrationBuilder.AddColumn<int>(
                name: "NameId",
                table: "PaymentTypes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTypes_NameId",
                table: "PaymentTypes",
                column: "NameId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTypes_textContents_NameId",
                table: "PaymentTypes",
                column: "NameId",
                principalTable: "textContents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTypes_textContents_NameId",
                table: "PaymentTypes");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTypes_NameId",
                table: "PaymentTypes");

            migrationBuilder.DropColumn(
                name: "NameId",
                table: "PaymentTypes");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "PaymentTypes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
