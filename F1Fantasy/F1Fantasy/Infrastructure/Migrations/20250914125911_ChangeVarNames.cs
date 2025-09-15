using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Fantasy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeVarNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_fantasy_lineup_application_user_owner_id",
                table: "fantasy_lineup");

            migrationBuilder.DropForeignKey(
                name: "fk_league_application_user_user_id",
                table: "league");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "league",
                newName: "owner_id");

            migrationBuilder.RenameIndex(
                name: "ix_league_user_id",
                table: "league",
                newName: "ix_league_owner_id");

            migrationBuilder.RenameColumn(
                name: "owner_id",
                table: "fantasy_lineup",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "ix_fantasy_lineup_owner_id",
                table: "fantasy_lineup",
                newName: "ix_fantasy_lineup_user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_fantasy_lineup_application_user_user_id",
                table: "fantasy_lineup",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_league_application_user_owner_id",
                table: "league",
                column: "owner_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_fantasy_lineup_application_user_user_id",
                table: "fantasy_lineup");

            migrationBuilder.DropForeignKey(
                name: "fk_league_application_user_owner_id",
                table: "league");

            migrationBuilder.RenameColumn(
                name: "owner_id",
                table: "league",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "ix_league_owner_id",
                table: "league",
                newName: "ix_league_user_id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "fantasy_lineup",
                newName: "owner_id");

            migrationBuilder.RenameIndex(
                name: "ix_fantasy_lineup_user_id",
                table: "fantasy_lineup",
                newName: "ix_fantasy_lineup_owner_id");

            migrationBuilder.AddForeignKey(
                name: "fk_fantasy_lineup_application_user_owner_id",
                table: "fantasy_lineup",
                column: "owner_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_league_application_user_user_id",
                table: "league",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
