using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace blandus_backend.Migrations
{
    public partial class AccommodationMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LocationName",
                table: "Accommodations",
                newName: "Name");

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "Accommodations",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Accommodations");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Accommodations",
                newName: "LocationName");
        }
    }
}
