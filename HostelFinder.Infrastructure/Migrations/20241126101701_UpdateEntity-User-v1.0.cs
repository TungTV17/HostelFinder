using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HostelFinder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntityUserv10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentDetails",
                table: "UserMemberships");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "UserMemberships");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "UserMemberships");

            migrationBuilder.AddColumn<long>(
                name: "TransactionId",
                table: "Users",
                type: "bigint",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WalletBalance",
                table: "Users",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "WalletBalance",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "PaymentDetails",
                table: "UserMemberships",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "UserMemberships",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "UserMemberships",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
