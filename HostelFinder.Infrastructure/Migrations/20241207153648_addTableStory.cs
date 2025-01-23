using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HostelFinder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addTableStory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StoryId",
                table: "Images",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoryId",
                table: "Addresses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Storys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    MonthlyRentCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: false),
                    bookingStatus = table.Column<int>(type: "int", nullable: false),
                    DateAvailable = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Storys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Storys_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Images_StoryId",
                table: "Images",
                column: "StoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_StoryId",
                table: "Addresses",
                column: "StoryId",
                unique: true,
                filter: "[StoryId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Storys_UserId",
                table: "Storys",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Storys_StoryId",
                table: "Addresses",
                column: "StoryId",
                principalTable: "Storys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Storys_StoryId",
                table: "Images",
                column: "StoryId",
                principalTable: "Storys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Storys_StoryId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Storys_StoryId",
                table: "Images");

            migrationBuilder.DropTable(
                name: "Storys");

            migrationBuilder.DropIndex(
                name: "IX_Images_StoryId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_StoryId",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "StoryId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "StoryId",
                table: "Addresses");
        }
    }
}
