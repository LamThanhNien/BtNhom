using ASC.Web.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASC.Web.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260427120000_Lab6_MasterDataKeyIsActive")]
    /// <inheritdoc />
    public partial class Lab6_MasterDataKeyIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "MasterDataKeys",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "MasterDataKeys");
        }
    }
}
