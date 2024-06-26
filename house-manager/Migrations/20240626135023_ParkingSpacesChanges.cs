using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace house_manager.Migrations
{
    /// <inheritdoc />
    public partial class ParkingSpacesChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParkingSpaces_Lodgers_OwnerId",
                table: "ParkingSpaces");

            migrationBuilder.DropIndex(
                name: "IX_ParkingSpaces_OwnerId",
                table: "ParkingSpaces");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "ParkingSpaces");

            migrationBuilder.CreateTable(
                name: "OwnedParkingSpaces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParkingSpaceId = table.Column<int>(type: "int", nullable: false),
                    OwnerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnedParkingSpaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OwnedParkingSpaces_Lodgers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Lodgers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OwnedParkingSpaces_ParkingSpaces_ParkingSpaceId",
                        column: x => x.ParkingSpaceId,
                        principalTable: "ParkingSpaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OwnedParkingSpaces_OwnerId",
                table: "OwnedParkingSpaces",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnedParkingSpaces_ParkingSpaceId",
                table: "OwnedParkingSpaces",
                column: "ParkingSpaceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OwnedParkingSpaces");

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "ParkingSpaces",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSpaces_OwnerId",
                table: "ParkingSpaces",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ParkingSpaces_Lodgers_OwnerId",
                table: "ParkingSpaces",
                column: "OwnerId",
                principalTable: "Lodgers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
