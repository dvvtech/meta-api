using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetaApi.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class delPromocode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FittingResults_Promocodes_PromocodeId",
                table: "FittingResults");

            migrationBuilder.RenameColumn(
                name: "PromocodeId",
                table: "FittingResults",
                newName: "AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_FittingResults_PromocodeId",
                table: "FittingResults",
                newName: "IX_FittingResults_AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_FittingResults_Accounts_AccountId",
                table: "FittingResults",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FittingResults_Accounts_AccountId",
                table: "FittingResults");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "FittingResults",
                newName: "PromocodeId");

            migrationBuilder.RenameIndex(
                name: "IX_FittingResults_AccountId",
                table: "FittingResults",
                newName: "IX_FittingResults_PromocodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_FittingResults_Promocodes_PromocodeId",
                table: "FittingResults",
                column: "PromocodeId",
                principalTable: "Promocodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
