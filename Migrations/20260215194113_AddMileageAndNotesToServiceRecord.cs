using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarServiceTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddMileageAndNotesToServiceRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Mileage",
                table: "ServiceRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "ServiceRecords",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mileage",
                table: "ServiceRecords");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "ServiceRecords");
        }
    }
}
