using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModularApp.Modules.Workspace.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CharacterMetadatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedByEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedByEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Characters = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Source = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsMigrated = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Html = table.Column<string>(type: "nvarchar(max)", maxLength: 10000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterMetadatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedByEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedByEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Characters = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MeaningMnemonic = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    MeaningMnemonicHint = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    MeaningExplanation = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    MeaningExplanationHint = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReadingMnemonic = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReadingMnemonicHint = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReadingExplanation = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReadingExplanationHint = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    LessonPosition = table.Column<int>(type: "int", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ExternalUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MeaningMnemonicOriginal = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    MeaningMnemonicHintOriginal = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    MeaningExplanationOriginal = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    MeaningExplanationHintOriginal = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReadingMnemonicOriginal = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReadingMnemonicHintOriginal = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReadingExplanationOriginal = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ReadingExplanationHintOriginal = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContextSentences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    En = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ja = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    CharacterId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContextSentences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContextSentences_Characters_CharacterId1",
                        column: x => x.CharacterId1,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Meanings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    WordType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    CharacterId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meanings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Meanings_Characters_CharacterId1",
                        column: x => x.CharacterId1,
                        principalTable: "Characters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Readings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    CharacterId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Readings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Readings_Characters_CharacterId1",
                        column: x => x.CharacterId1,
                        principalTable: "Characters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterMetadatas_Characters_Source",
                table: "CharacterMetadatas",
                columns: new[] { "Characters", "Source" });

            migrationBuilder.CreateIndex(
                name: "IX_Characters_Characters_Type_Source",
                table: "Characters",
                columns: new[] { "Characters", "Type", "Source" });

            migrationBuilder.CreateIndex(
                name: "IX_ContextSentences_CharacterId1",
                table: "ContextSentences",
                column: "CharacterId1");

            migrationBuilder.CreateIndex(
                name: "IX_Meanings_CharacterId1",
                table: "Meanings",
                column: "CharacterId1");

            migrationBuilder.CreateIndex(
                name: "IX_Readings_CharacterId1",
                table: "Readings",
                column: "CharacterId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterMetadatas");

            migrationBuilder.DropTable(
                name: "ContextSentences");

            migrationBuilder.DropTable(
                name: "Meanings");

            migrationBuilder.DropTable(
                name: "Readings");

            migrationBuilder.DropTable(
                name: "Characters");
        }
    }
}
