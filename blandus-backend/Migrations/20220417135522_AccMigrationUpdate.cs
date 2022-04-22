using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace blandus_backend.Migrations
{
    public partial class AccMigrationUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Location",
                table: "Accommodations",
                newName: "RoomType");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Accommodations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Accommodations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Accommodations");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Accommodations");

            migrationBuilder.RenameColumn(
                name: "RoomType",
                table: "Accommodations",
                newName: "Location");
        }
    }
}
