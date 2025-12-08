using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Fantasy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeConstructorCountryIdNullable : Migration
    {
        /// <inheritdoc />
 protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the foreign key constraint
            migrationBuilder.Sql(@"
                ALTER TABLE constructor 
                DROP CONSTRAINT IF EXISTS fk_constructor_country_country_id;
            ");

            // Remove the default value and make nullable
            migrationBuilder.Sql(@"
                ALTER TABLE constructor 
                ALTER COLUMN country_id DROP DEFAULT;
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE constructor 
                ALTER COLUMN country_id DROP NOT NULL;
            ");

            // Update existing empty strings to NULL
            migrationBuilder.Sql(@"
                UPDATE constructor 
                SET country_id = NULL 
                WHERE country_id = '';
            ");

            // Re-add the foreign key constraint (allowing NULL)
            migrationBuilder.Sql(@"
                ALTER TABLE constructor 
                ADD CONSTRAINT fk_constructor_country_country_id 
                FOREIGN KEY (country_id) 
                REFERENCES country(id) 
                ON DELETE SET NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE constructor 
                DROP CONSTRAINT IF EXISTS fk_constructor_country_country_id;
            ");

            migrationBuilder.Sql(@"
                UPDATE constructor 
                SET country_id = '' 
                WHERE country_id IS NULL;
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE constructor 
                ALTER COLUMN country_id SET DEFAULT '';
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE constructor 
                ALTER COLUMN country_id SET NOT NULL;
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE constructor 
                ADD CONSTRAINT fk_constructor_country_country_id 
                FOREIGN KEY (country_id) 
                REFERENCES country(id);
            ");
        }
    }
}
