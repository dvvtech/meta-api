using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetaApi.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Promocodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Promocode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UsageLimit = table.Column<int>(type: "int", nullable: false),
                    RemainingUsage = table.Column<int>(type: "int", nullable: false),
                    CreatedUtcDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateUtcDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promocodes", x => x.Id);
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
                    PromocodeId = table.Column<int>(type: "int", nullable: false),
                    CreatedUtcDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FittingResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FittingResults_Promocodes_PromocodeId",
                        column: x => x.PromocodeId,
                        principalTable: "Promocodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Promocodes",
                columns: new[] { "Id", "CreatedUtcDate", "Name", "Promocode", "RemainingUsage", "UpdateUtcDate", "UsageLimit" },
                values: new object[] { 1, new DateTime(2024, 12, 17, 7, 50, 37, 636, DateTimeKind.Utc).AddTicks(3506), "admin", "PRBA34YNI9!QWC7IZS", 500000, new DateTime(2024, 12, 17, 7, 50, 37, 636, DateTimeKind.Utc).AddTicks(3506), 500000 });

            migrationBuilder.CreateIndex(
                name: "IX_FittingResults_PromocodeId",
                table: "FittingResults",
                column: "PromocodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Promocodes_Promocode",
                table: "Promocodes",
                column: "Promocode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FittingResults");

            migrationBuilder.DropTable(
                name: "Promocodes");
        }
    }
}
