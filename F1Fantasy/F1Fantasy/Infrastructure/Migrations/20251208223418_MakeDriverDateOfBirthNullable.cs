using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Fantasy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeDriverDateOfBirthNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove the default value
            migrationBuilder.Sql(@"
                ALTER TABLE driver 
                ALTER COLUMN date_of_birth DROP DEFAULT;
            ");

            // Make the column nullable
            migrationBuilder.Sql(@"
                ALTER TABLE driver 
                ALTER COLUMN date_of_birth DROP NOT NULL;
            ");

            // Update existing '-infinity' values to NULL
            migrationBuilder.Sql(@"
                UPDATE driver 
                SET date_of_birth = NULL 
                WHERE date_of_birth = '-infinity'::date;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert NULL values back to '-infinity'
            migrationBuilder.Sql(@"
                UPDATE driver 
                SET date_of_birth = '-infinity'::date 
                WHERE date_of_birth IS NULL;
            ");

            // Make the column NOT NULL
            migrationBuilder.Sql(@"
                ALTER TABLE driver 
                ALTER COLUMN date_of_birth SET NOT NULL;
            ");

            // Add the default value back
            migrationBuilder.Sql(@"
                ALTER TABLE driver 
                ALTER COLUMN date_of_birth SET DEFAULT '-infinity'::date;
            ");
        }
    }
}
