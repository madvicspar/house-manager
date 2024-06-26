using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace house_manager.Migrations
{
    /// <inheritdoc />
    public partial class ParkingSpacesChanges2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "ParkingSpaces",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSpaces_Number",
                table: "ParkingSpaces",
                column: "Number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ParkingSpaces_Number",
                table: "ParkingSpaces");

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "ParkingSpaces",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
