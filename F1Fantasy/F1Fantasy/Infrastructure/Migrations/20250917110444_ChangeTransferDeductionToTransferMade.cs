using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Fantasy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTransferDeductionToTransferMade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "transfer_points_deducted",
                table: "fantasy_lineup",
                newName: "transfers_made");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "transfers_made",
                table: "fantasy_lineup",
                newName: "transfer_points_deducted");
        }
    }
}
