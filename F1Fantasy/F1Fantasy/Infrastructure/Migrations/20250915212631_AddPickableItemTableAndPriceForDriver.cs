using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Fantasy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPickableItemTableAndPriceForDriver : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "pickable_item_id",
                table: "driver",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "price",
                table: "driver",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "pickable_item_id",
                table: "constructor",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "pickable_item",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pickable_item", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_driver_pickable_item_id",
                table: "driver",
                column: "pickable_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_constructor_pickable_item_id",
                table: "constructor",
                column: "pickable_item_id");

            migrationBuilder.AddForeignKey(
                name: "fk_constructor_pickable_item_pickable_item_id",
                table: "constructor",
                column: "pickable_item_id",
                principalTable: "pickable_item",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_driver_pickable_item_pickable_item_id",
                table: "driver",
                column: "pickable_item_id",
                principalTable: "pickable_item",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_constructor_pickable_item_pickable_item_id",
                table: "constructor");

            migrationBuilder.DropForeignKey(
                name: "fk_driver_pickable_item_pickable_item_id",
                table: "driver");

            migrationBuilder.DropTable(
                name: "pickable_item");

            migrationBuilder.DropIndex(
                name: "ix_driver_pickable_item_id",
                table: "driver");

            migrationBuilder.DropIndex(
                name: "ix_constructor_pickable_item_id",
                table: "constructor");

            migrationBuilder.DropColumn(
                name: "pickable_item_id",
                table: "driver");

            migrationBuilder.DropColumn(
                name: "price",
                table: "driver");

            migrationBuilder.DropColumn(
                name: "pickable_item_id",
                table: "constructor");
        }
    }
}
