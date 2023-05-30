using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaMarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class rename_actions_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActionModel_textContents_DescriptionId",
                table: "ActionModel");

            migrationBuilder.DropForeignKey(
                name: "FK_ActionModel_textContents_NameId",
                table: "ActionModel");

            migrationBuilder.DropForeignKey(
                name: "FK_ActionModelArticleModel_ActionModel_ActionsId",
                table: "ActionModelArticleModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActionModel",
                table: "ActionModel");

            migrationBuilder.RenameTable(
                name: "ActionModel",
                newName: "Actions");

            migrationBuilder.RenameIndex(
                name: "IX_ActionModel_NameId",
                table: "Actions",
                newName: "IX_Actions_NameId");

            migrationBuilder.RenameIndex(
                name: "IX_ActionModel_DescriptionId",
                table: "Actions",
                newName: "IX_Actions_DescriptionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Actions",
                table: "Actions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActionModelArticleModel_Actions_ActionsId",
                table: "ActionModelArticleModel",
                column: "ActionsId",
                principalTable: "Actions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_textContents_DescriptionId",
                table: "Actions",
                column: "DescriptionId",
                principalTable: "textContents",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_textContents_NameId",
                table: "Actions",
                column: "NameId",
                principalTable: "textContents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActionModelArticleModel_Actions_ActionsId",
                table: "ActionModelArticleModel");

            migrationBuilder.DropForeignKey(
                name: "FK_Actions_textContents_DescriptionId",
                table: "Actions");

            migrationBuilder.DropForeignKey(
                name: "FK_Actions_textContents_NameId",
                table: "Actions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Actions",
                table: "Actions");

            migrationBuilder.RenameTable(
                name: "Actions",
                newName: "ActionModel");

            migrationBuilder.RenameIndex(
                name: "IX_Actions_NameId",
                table: "ActionModel",
                newName: "IX_ActionModel_NameId");

            migrationBuilder.RenameIndex(
                name: "IX_Actions_DescriptionId",
                table: "ActionModel",
                newName: "IX_ActionModel_DescriptionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActionModel",
                table: "ActionModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActionModel_textContents_DescriptionId",
                table: "ActionModel",
                column: "DescriptionId",
                principalTable: "textContents",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActionModel_textContents_NameId",
                table: "ActionModel",
                column: "NameId",
                principalTable: "textContents",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActionModelArticleModel_ActionModel_ActionsId",
                table: "ActionModelArticleModel",
                column: "ActionsId",
                principalTable: "ActionModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
