using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uploadify.Server.Data.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSchemas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSchema("Files");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
