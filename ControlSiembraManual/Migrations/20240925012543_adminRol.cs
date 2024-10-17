using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionSiembra.Migrations
{
    /// <inheritdoc />
    public partial class adminRol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            IF NOT EXISTS(Select Id From AspNetRoles where Id = '48d18940-2af5-4f7a-b64c-6c531909a2d4')
            BEGIN
              INSERT AspNetRoles (Id, [Name],[NormalizedName])
              VALUES ('48d18940-2af5-4f7a-b64c-6c531909a2d4','admin','ADMIN')
            END
             ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(" DELETE AspNetRoles WHERE Id ='48d18940-2af5-4f7a-b64c-6c531909a2d4'");
        }
    }
}
