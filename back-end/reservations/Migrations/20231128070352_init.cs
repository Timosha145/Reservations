using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace reservations.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Serviceid",
                table: "Reservations",
                newName: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "Reservations",
                newName: "Serviceid");
        }
    }
}
