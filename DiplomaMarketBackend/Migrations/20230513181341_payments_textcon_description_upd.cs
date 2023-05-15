using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class payments_textcon_description_upd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTypes_PaymentTypes_ParentId",
                table: "PaymentTypes");

            migrationBuilder.AlterColumn<int>(
                name: "ParentId",
                table: "PaymentTypes",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTypes_PaymentTypes_ParentId",
                table: "PaymentTypes",
                column: "ParentId",
                principalTable: "PaymentTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTypes_PaymentTypes_ParentId",
                table: "PaymentTypes");

            migrationBuilder.AlterColumn<int>(
                name: "ParentId",
                table: "PaymentTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTypes_PaymentTypes_ParentId",
                table: "PaymentTypes",
                column: "ParentId",
                principalTable: "PaymentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
