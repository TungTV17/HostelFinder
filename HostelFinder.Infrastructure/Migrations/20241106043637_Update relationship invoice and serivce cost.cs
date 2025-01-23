using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HostelFinder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Updaterelationshipinvoiceandserivcecost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceCosts_InVoices_InVoiceId",
                table: "ServiceCosts");

            migrationBuilder.DropIndex(
                name: "IX_ServiceCosts_InVoiceId",
                table: "ServiceCosts");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "ServiceCosts");

            migrationBuilder.DropColumn(
                name: "CurrentReading",
                table: "ServiceCosts");

            migrationBuilder.DropColumn(
                name: "PreviousReading",
                table: "ServiceCosts");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "InVoices");

            migrationBuilder.RenameColumn(
                name: "IsCharge",
                table: "Services",
                newName: "IsUsageBased");

            migrationBuilder.RenameColumn(
                name: "InVoiceId",
                table: "ServiceCosts",
                newName: "InvoiceId");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Rooms",
                newName: "IsAvailable");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "InVoices",
                newName: "IsPaid");

            migrationBuilder.RenameColumn(
                name: "ServiceCostId",
                table: "InVoices",
                newName: "RoomId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Services",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBillable",
                table: "Services",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<decimal>(
                name: "Size",
                table: "Rooms",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Rooms",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BillingMonth",
                table: "InVoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BillingYear",
                table: "InVoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "InvoiceDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActualCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NumberOfCustomer = table.Column<int>(type: "int", nullable: true),
                    PreviousReading = table.Column<int>(type: "int", nullable: false),
                    CurrentReading = table.Column<int>(type: "int", nullable: false),
                    BillingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceDetail_InVoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "InVoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceDetail_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InVoices_RoomId",
                table: "InVoices",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetail_InvoiceId",
                table: "InvoiceDetail",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetail_ServiceId",
                table: "InvoiceDetail",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_InVoices_Rooms_RoomId",
                table: "InVoices",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InVoices_Rooms_RoomId",
                table: "InVoices");

            migrationBuilder.DropTable(
                name: "InvoiceDetail");

            migrationBuilder.DropIndex(
                name: "IX_InVoices_RoomId",
                table: "InVoices");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "IsBillable",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "BillingMonth",
                table: "InVoices");

            migrationBuilder.DropColumn(
                name: "BillingYear",
                table: "InVoices");

            migrationBuilder.RenameColumn(
                name: "IsUsageBased",
                table: "Services",
                newName: "IsCharge");

            migrationBuilder.RenameColumn(
                name: "InvoiceId",
                table: "ServiceCosts",
                newName: "InVoiceId");

            migrationBuilder.RenameColumn(
                name: "IsAvailable",
                table: "Rooms",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "RoomId",
                table: "InVoices",
                newName: "ServiceCostId");

            migrationBuilder.RenameColumn(
                name: "IsPaid",
                table: "InVoices",
                newName: "Status");

            migrationBuilder.AddColumn<decimal>(
                name: "Cost",
                table: "ServiceCosts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "CurrentReading",
                table: "ServiceCosts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PreviousReading",
                table: "ServiceCosts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<float>(
                name: "Size",
                table: "Rooms",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "InVoices",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCosts_InVoiceId",
                table: "ServiceCosts",
                column: "InVoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceCosts_InVoices_InVoiceId",
                table: "ServiceCosts",
                column: "InVoiceId",
                principalTable: "InVoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
