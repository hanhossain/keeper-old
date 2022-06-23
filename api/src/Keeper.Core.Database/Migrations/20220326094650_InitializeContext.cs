using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Keeper.Core.Database.Migrations
{
    public partial class InitializeContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NflPlayers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Team = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NflPlayers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SleeperPlayers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Team = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SleeperPlayers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NflDefensiveStatistics",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    Season = table.Column<int>(type: "int", nullable: false),
                    Week = table.Column<int>(type: "int", nullable: false),
                    Sacks = table.Column<int>(type: "int", nullable: true),
                    Interceptions = table.Column<int>(type: "int", nullable: true),
                    FumblesRecovered = table.Column<int>(type: "int", nullable: true),
                    Safeties = table.Column<int>(type: "int", nullable: true),
                    Touchdowns = table.Column<int>(type: "int", nullable: true),
                    Def2PtRet = table.Column<int>(type: "int", nullable: true),
                    RetTouchdowns = table.Column<int>(type: "int", nullable: true),
                    PointsAllowed = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NflDefensiveStatistics", x => new { x.PlayerId, x.Season, x.Week });
                    table.ForeignKey(
                        name: "FK_NflDefensiveStatistics_NflPlayers_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "NflPlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NflKickingStatistics",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    Season = table.Column<int>(type: "int", nullable: false),
                    Week = table.Column<int>(type: "int", nullable: false),
                    PatMade = table.Column<int>(type: "int", nullable: true),
                    FieldGoal0To19Yards = table.Column<int>(type: "int", nullable: true),
                    FieldGoal20To29Yards = table.Column<int>(type: "int", nullable: true),
                    FieldGoal30To39Yards = table.Column<int>(type: "int", nullable: true),
                    FieldGoal40To49Yards = table.Column<int>(type: "int", nullable: true),
                    FieldGoal50PlusYards = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NflKickingStatistics", x => new { x.PlayerId, x.Season, x.Week });
                    table.ForeignKey(
                        name: "FK_NflKickingStatistics_NflPlayers_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "NflPlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NflOffensiveStatistics",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    Season = table.Column<int>(type: "int", nullable: false),
                    Week = table.Column<int>(type: "int", nullable: false),
                    PassingYards = table.Column<int>(type: "int", nullable: true),
                    PassingTouchdowns = table.Column<int>(type: "int", nullable: true),
                    PassingInterceptions = table.Column<int>(type: "int", nullable: true),
                    RushingYards = table.Column<int>(type: "int", nullable: true),
                    RushingTouchdowns = table.Column<int>(type: "int", nullable: true),
                    ReceivingReceptions = table.Column<int>(type: "int", nullable: true),
                    ReceivingYards = table.Column<int>(type: "int", nullable: true),
                    ReceivingTouchdowns = table.Column<int>(type: "int", nullable: true),
                    ReturningTouchdowns = table.Column<int>(type: "int", nullable: true),
                    TwoPointConversions = table.Column<int>(type: "int", nullable: true),
                    FumbleTouchdowns = table.Column<int>(type: "int", nullable: true),
                    FumblesLost = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NflOffensiveStatistics", x => new { x.PlayerId, x.Season, x.Week });
                    table.ForeignKey(
                        name: "FK_NflOffensiveStatistics_NflPlayers_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "NflPlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NflPlayerStatistics",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    Season = table.Column<int>(type: "int", nullable: false),
                    Week = table.Column<int>(type: "int", nullable: false),
                    FantasyPoints = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NflPlayerStatistics", x => new { x.PlayerId, x.Season, x.Week });
                    table.ForeignKey(
                        name: "FK_NflPlayerStatistics_NflPlayers_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "NflPlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NflDefensiveStatistics");

            migrationBuilder.DropTable(
                name: "NflKickingStatistics");

            migrationBuilder.DropTable(
                name: "NflOffensiveStatistics");

            migrationBuilder.DropTable(
                name: "NflPlayerStatistics");

            migrationBuilder.DropTable(
                name: "SleeperPlayers");

            migrationBuilder.DropTable(
                name: "NflPlayers");
        }
    }
}
