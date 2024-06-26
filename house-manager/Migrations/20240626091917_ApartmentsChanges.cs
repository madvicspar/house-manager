using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace house_manager.Migrations
{
    /// <inheritdoc />
    public partial class ApartmentsChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResidentsNumber",
                table: "OwnedApartments");

            migrationBuilder.AddColumn<int>(
                name: "ResidentsNumber",
                table: "Apartments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResidentsNumber",
                table: "Apartments");

            migrationBuilder.AddColumn<int>(
                name: "ResidentsNumber",
                table: "OwnedApartments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
