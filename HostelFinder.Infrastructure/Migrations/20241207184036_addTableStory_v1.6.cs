using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HostelFinder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addTableStory_v16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Hostels_HostelId",
                table: "Addresses");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Hostels_HostelId",
                table: "Addresses",
                column: "HostelId",
                principalTable: "Hostels",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Hostels_HostelId",
                table: "Addresses");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Hostels_HostelId",
                table: "Addresses",
                column: "HostelId",
                principalTable: "Hostels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
