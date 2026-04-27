using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Web.Migrations
{
    /// <inheritdoc />
    public partial class Lab7_ServiceRequestModelSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RequestName",
                table: "ServiceRequests",
                newName: "VehicleType");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "ServiceRequests",
                newName: "VehicleName");

            migrationBuilder.RenameColumn(
                name: "AssignedTo",
                table: "ServiceRequests",
                newName: "RequestedServices");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedDate",
                table: "ServiceRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartitionKey",
                table: "ServiceRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestedDate",
                table: "ServiceRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceEngineer",
                table: "ServiceRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedDate",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "PartitionKey",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "RequestedDate",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "ServiceEngineer",
                table: "ServiceRequests");

            migrationBuilder.RenameColumn(
                name: "VehicleType",
                table: "ServiceRequests",
                newName: "RequestName");

            migrationBuilder.RenameColumn(
                name: "VehicleName",
                table: "ServiceRequests",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "RequestedServices",
                table: "ServiceRequests",
                newName: "AssignedTo");
        }
    }
}
