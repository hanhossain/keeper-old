using Microsoft.EntityFrameworkCore.Migrations;

namespace Keeper.Core.Migrations
{
    public partial class AddKickingStatistics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NflKickingStatistics",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Season = table.Column<int>(type: "INTEGER", nullable: false),
                    Week = table.Column<int>(type: "INTEGER", nullable: false),
                    PatMade = table.Column<int>(type: "INTEGER", nullable: true),
                    FieldGoal0To19Yards = table.Column<int>(type: "INTEGER", nullable: true),
                    FieldGoal20To29Yards = table.Column<int>(type: "INTEGER", nullable: true),
                    FieldGoal30To39Yards = table.Column<int>(type: "INTEGER", nullable: true),
                    FieldGoal40To49Yards = table.Column<int>(type: "INTEGER", nullable: true),
                    FieldGoal50PlusYards = table.Column<int>(type: "INTEGER", nullable: true)
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NflKickingStatistics");
        }
    }
}
