using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace blandus_backend.Migrations
{
    public partial class AddRatingAndStars : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Rating",
                table: "Accommodations",
                type: "decimal(3,1)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "StarRating",
                table: "Accommodations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Accommodations");

            migrationBuilder.DropColumn(
                name: "StarRating",
                table: "Accommodations");
        }
    }
}
