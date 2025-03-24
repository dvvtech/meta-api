using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetaApi.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class addUserTable5 : Migration
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
