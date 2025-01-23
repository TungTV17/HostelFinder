using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HostelFinder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Updateentityservicecost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceCosts_Rooms_RoomId",
                table: "ServiceCosts");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "ServiceCosts");

            migrationBuilder.RenameColumn(
                name: "RoomId",
                table: "ServiceCosts",
                newName: "HostelId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceCosts_RoomId",
                table: "ServiceCosts",
                newName: "IX_ServiceCosts_HostelId");

            migrationBuilder.RenameColumn(
                name: "commune",
                table: "Addresses",
                newName: "Commune");

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveFrom",
                table: "ServiceCosts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveTo",
                table: "ServiceCosts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceCosts_Hostels_HostelId",
                table: "ServiceCosts",
                column: "HostelId",
                principalTable: "Hostels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceCosts_Hostels_HostelId",
                table: "ServiceCosts");

            migrationBuilder.DropColumn(
                name: "EffectiveFrom",
                table: "ServiceCosts");

            migrationBuilder.DropColumn(
                name: "EffectiveTo",
                table: "ServiceCosts");

            migrationBuilder.RenameColumn(
                name: "HostelId",
                table: "ServiceCosts",
                newName: "RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceCosts_HostelId",
                table: "ServiceCosts",
                newName: "IX_ServiceCosts_RoomId");

            migrationBuilder.RenameColumn(
                name: "Commune",
                table: "Addresses",
                newName: "commune");

            migrationBuilder.AddColumn<Guid>(
                name: "InvoiceId",
                table: "ServiceCosts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceCosts_Rooms_RoomId",
                table: "ServiceCosts",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
