using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace Uploadify.Server.Data.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddedSearchVectorToFilesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "SearchVector",
                schema: "Files",
                table: "Files",
                type: "tsvector",
                nullable: false)
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "UnsafeName", "Extension", "MimeType" });

            migrationBuilder.CreateIndex(
                name: "IX_Files_SearchVector",
                schema: "Files",
                table: "Files",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Files_SearchVector",
                schema: "Files",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "SearchVector",
                schema: "Files",
                table: "Files");
        }
    }
}
