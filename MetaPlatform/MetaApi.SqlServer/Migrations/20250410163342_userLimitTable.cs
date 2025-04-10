using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetaApi.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class userLimitTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FittingResults_AccountId",
                table: "FittingResults");

            migrationBuilder.CreateTable(
                name: "UserTryOnLimits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    MaxAttempts = table.Column<int>(type: "int", nullable: false, defaultValue: 3),
                    AttemptsUsed = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastResetTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResetPeriod = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTryOnLimits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTryOnLimits_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FittingResults_AccountId",
                table: "FittingResults",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTryOnLimits_AccountId",
                table: "UserTryOnLimits",
                column: "AccountId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserTryOnLimits");

            migrationBuilder.DropIndex(
                name: "IX_FittingResults_AccountId",
                table: "FittingResults");

            migrationBuilder.CreateIndex(
                name: "IX_FittingResults_AccountId",
                table: "FittingResults",
                column: "AccountId");
        }
    }
}
