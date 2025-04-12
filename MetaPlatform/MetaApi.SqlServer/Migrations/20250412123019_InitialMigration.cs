using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetaApi.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    JwtRefreshToken = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    AuthType = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    IsBlocked = table.Column<bool>(type: "bit", nullable: false),
                    CreatedUtcDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateUtcDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FittingResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GarmentImgUrl = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    HumanImgUrl = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ResultImgUrl = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    CreatedUtcDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FittingResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FittingResults_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_Accounts_ExternalId",
                table: "Accounts",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_JwtRefreshToken",
                table: "Accounts",
                column: "JwtRefreshToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FittingResults_AccountId",
                table: "FittingResults",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTryOnLimits_AccountId",
                table: "UserTryOnLimits",
                column: "AccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FittingResults");

            migrationBuilder.DropTable(
                name: "UserTryOnLimits");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
