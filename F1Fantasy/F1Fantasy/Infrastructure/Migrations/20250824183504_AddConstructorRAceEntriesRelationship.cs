using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Fantasy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConstructorRAceEntriesRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "constructor_id",
                table: "race_entry",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_race_entry_constructor_id",
                table: "race_entry",
                column: "constructor_id");

            migrationBuilder.AddForeignKey(
                name: "fk_race_entry_constructor_constructor_id",
                table: "race_entry",
                column: "constructor_id",
                principalTable: "constructor",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_race_entry_constructor_constructor_id",
                table: "race_entry");

            migrationBuilder.DropIndex(
                name: "ix_race_entry_constructor_id",
                table: "race_entry");

            migrationBuilder.DropColumn(
                name: "constructor_id",
                table: "race_entry");
        }
    }
}
