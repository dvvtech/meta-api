using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetaApi.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class addTotalAttemptsUsed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalAttemptsUsed",
                table: "UserTryOnLimits",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAttemptsUsed",
                table: "UserTryOnLimits");
        }
    }
}
