using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HostelFinder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addTableStory_v11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "bookingStatus",
                table: "Storys",
                newName: "BookingStatus");

            migrationBuilder.AddColumn<int>(
                name: "RoomType",
                table: "Storys",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Size",
                table: "Storys",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoomType",
                table: "Storys");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Storys");

            migrationBuilder.RenameColumn(
                name: "BookingStatus",
                table: "Storys",
                newName: "bookingStatus");
        }
    }
}
