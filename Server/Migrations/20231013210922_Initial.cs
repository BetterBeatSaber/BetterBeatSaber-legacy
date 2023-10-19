using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace BetterBeatSaber.Server.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Bans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    SteamId = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    DiscordId = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    BeatSaverId = table.Column<ulong>(type: "bigint unsigned", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bans", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    AvatarUrl = table.Column<string>(type: "longtext", nullable: false),
                    Role = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Flags = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    ScoreSaberPp = table.Column<double>(type: "double", nullable: true),
                    ScoreSaberGlobalRank = table.Column<uint>(type: "int unsigned", nullable: true),
                    ScoreSaberCountryRank = table.Column<uint>(type: "int unsigned", nullable: true),
                    ScoreSaberCountry = table.Column<string>(type: "longtext", nullable: true),
                    BeatLeaderPp = table.Column<double>(type: "double", nullable: true),
                    BeatLeaderGlobalRank = table.Column<uint>(type: "int unsigned", nullable: true),
                    BeatLeaderCountryRank = table.Column<uint>(type: "int unsigned", nullable: true),
                    BeatLeaderCountry = table.Column<string>(type: "longtext", nullable: true),
                    LastUpdate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlayerIntegrations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    PlayerId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Type = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    AccessToken = table.Column<byte[]>(type: "longblob", nullable: false),
                    RefreshToken = table.Column<byte[]>(type: "longblob", nullable: false),
                    TokenType = table.Column<string>(type: "longtext", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerIntegrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerIntegrations_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlayerRelationships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    FirstPlayerId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    SecondPlayerId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    RelationshipType = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerRelationships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerRelationships_Players_FirstPlayerId",
                        column: x => x.FirstPlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerRelationships_Players_SecondPlayerId",
                        column: x => x.SecondPlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerIntegrations_PlayerId",
                table: "PlayerIntegrations",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerRelationships_FirstPlayerId",
                table: "PlayerRelationships",
                column: "FirstPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerRelationships_SecondPlayerId",
                table: "PlayerRelationships",
                column: "SecondPlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bans");

            migrationBuilder.DropTable(
                name: "PlayerIntegrations");

            migrationBuilder.DropTable(
                name: "PlayerRelationships");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
