using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elaaj.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class inition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsResolved",
                table: "Prescriptions");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Prescriptions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Prescriptions");

            migrationBuilder.AddColumn<bool>(
                name: "IsResolved",
                table: "Prescriptions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
