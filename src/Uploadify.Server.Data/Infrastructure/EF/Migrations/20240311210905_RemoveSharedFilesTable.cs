using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Uploadify.Server.Data.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSharedFilesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SharedFiles",
                schema: "FileSystem");

            migrationBuilder.EnsureSchema(
                name: "Files");

            migrationBuilder.RenameTable(
                name: "Folders",
                schema: "FileSystem",
                newName: "Folders",
                newSchema: "Files");

            migrationBuilder.RenameTable(
                name: "Files",
                schema: "FileSystem",
                newName: "Files",
                newSchema: "Files");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                schema: "Files",
                table: "Files",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Files_IsPublic",
                schema: "Files",
                table: "Files",
                column: "IsPublic");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Files_IsPublic",
                schema: "Files",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                schema: "Files",
                table: "Files");

            migrationBuilder.EnsureSchema(
                name: "FileSystem");

            migrationBuilder.RenameTable(
                name: "Folders",
                schema: "Files",
                newName: "Folders",
                newSchema: "FileSystem");

            migrationBuilder.RenameTable(
                name: "Files",
                schema: "Files",
                newName: "Files",
                newSchema: "FileSystem");

            migrationBuilder.CreateTable(
                name: "SharedFiles",
                schema: "FileSystem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    FileId = table.Column<int>(type: "integer", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharedFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SharedFiles_Files_FileId",
                        column: x => x.FileId,
                        principalSchema: "FileSystem",
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SharedFiles_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "Application",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SharedFiles_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalSchema: "Application",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SharedFiles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Application",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SharedFiles_CreatedBy",
                schema: "FileSystem",
                table: "SharedFiles",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SharedFiles_FileId",
                schema: "FileSystem",
                table: "SharedFiles",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_SharedFiles_IsActive",
                schema: "FileSystem",
                table: "SharedFiles",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_SharedFiles_UpdatedBy",
                schema: "FileSystem",
                table: "SharedFiles",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SharedFiles_UserId",
                schema: "FileSystem",
                table: "SharedFiles",
                column: "UserId");
        }
    }
}
