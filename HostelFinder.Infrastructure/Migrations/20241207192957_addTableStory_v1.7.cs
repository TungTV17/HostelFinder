using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HostelFinder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addTableStory_v17 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Hostels_HostelId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Storys_StoryId",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_StoryId",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "StoryId",
                table: "Addresses");

            migrationBuilder.CreateTable(
                name: "AddressStories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Province = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Commune = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DetailAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressStories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AddressStories_Storys_StoryId",
                        column: x => x.StoryId,
                        principalTable: "Storys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AddressStories_StoryId",
                table: "AddressStories",
                column: "StoryId",
                unique: true,
                filter: "[StoryId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Hostels_HostelId",
                table: "Addresses",
                column: "HostelId",
                principalTable: "Hostels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Hostels_HostelId",
                table: "Addresses");

            migrationBuilder.DropTable(
                name: "AddressStories");

            migrationBuilder.AddColumn<Guid>(
                name: "StoryId",
                table: "Addresses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_StoryId",
                table: "Addresses",
                column: "StoryId",
                unique: true,
                filter: "[StoryId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Hostels_HostelId",
                table: "Addresses",
                column: "HostelId",
                principalTable: "Hostels",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Storys_StoryId",
                table: "Addresses",
                column: "StoryId",
                principalTable: "Storys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
