using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elaaj.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAcceptedPharmacyId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<Guid>(
            //    name: "AcceptedPharmacyId",
            //    table: "Prescriptions",
            //    type: "uniqueidentifier",
            //    nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptedPharmacyId",
                table: "Prescriptions");
        }
    }
}
