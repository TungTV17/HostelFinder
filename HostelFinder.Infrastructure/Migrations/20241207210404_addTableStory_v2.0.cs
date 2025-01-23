using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HostelFinder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addTableStory_v20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AddressStories_Storys_StoryId",
                table: "AddressStories");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Storys_StoryId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Storys_Users_UserId",
                table: "Storys");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Storys",
                table: "Storys");

            migrationBuilder.RenameTable(
                name: "Storys",
                newName: "Stories");

            migrationBuilder.RenameIndex(
                name: "IX_Storys_UserId",
                table: "Stories",
                newName: "IX_Stories_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stories",
                table: "Stories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AddressStories_Stories_StoryId",
                table: "AddressStories",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Stories_StoryId",
                table: "Images",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stories_Users_UserId",
                table: "Stories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AddressStories_Stories_StoryId",
                table: "AddressStories");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Stories_StoryId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Stories_Users_UserId",
                table: "Stories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Stories",
                table: "Stories");

            migrationBuilder.RenameTable(
                name: "Stories",
                newName: "Storys");

            migrationBuilder.RenameIndex(
                name: "IX_Stories_UserId",
                table: "Storys",
                newName: "IX_Storys_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Storys",
                table: "Storys",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AddressStories_Storys_StoryId",
                table: "AddressStories",
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

            migrationBuilder.AddForeignKey(
                name: "FK_Storys_Users_UserId",
                table: "Storys",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
