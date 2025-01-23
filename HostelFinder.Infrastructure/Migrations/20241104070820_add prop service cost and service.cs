using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HostelFinder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addpropservicecostandservice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceName",
                table: "ServiceCosts");

            migrationBuilder.RenameColumn(
                name: "unitCost",
                table: "ServiceCosts",
                newName: "UnitCost");

            migrationBuilder.AddColumn<bool>(
                name: "IsCharge",
                table: "Services",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ServiceId",
                table: "ServiceCosts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCosts_ServiceId",
                table: "ServiceCosts",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceCosts_Services_ServiceId",
                table: "ServiceCosts",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceCosts_Services_ServiceId",
                table: "ServiceCosts");

            migrationBuilder.DropIndex(
                name: "IX_ServiceCosts_ServiceId",
                table: "ServiceCosts");

            migrationBuilder.DropColumn(
                name: "IsCharge",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "ServiceCosts");

            migrationBuilder.RenameColumn(
                name: "UnitCost",
                table: "ServiceCosts",
                newName: "unitCost");

            migrationBuilder.AddColumn<string>(
                name: "ServiceName",
                table: "ServiceCosts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
