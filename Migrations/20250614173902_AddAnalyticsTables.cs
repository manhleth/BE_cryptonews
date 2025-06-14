using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsPaper.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalyticsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyStats",
                columns: table => new
                {
                    DailyStatsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalPageViews = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    UniqueVisitors = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    NewUsers = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    NewPosts = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    NewComments = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyStats", x => x.DailyStatsId);
                });

            migrationBuilder.CreateTable(
                name: "NewsAnalytics",
                columns: table => new
                {
                    NewsAnalyticsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NewsId = table.Column<int>(type: "int", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    UniqueViewCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    SaveCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CommentCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    AverageReadTime = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false, defaultValue: 0m),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsAnalytics", x => x.NewsAnalyticsId);
                    table.ForeignKey(
                        name: "FK_NewsAnalytics_News_NewsId",
                        column: x => x.NewsId,
                        principalTable: "News",
                        principalColumn: "NewsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageViews",
                columns: table => new
                {
                    PageViewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    NewsId = table.Column<int>(type: "int", nullable: true),
                    PageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PageTitle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SessionDuration = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ViewDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageViews", x => x.PageViewId);
                    table.ForeignKey(
                        name: "FK_PageViews_News_NewsId",
                        column: x => x.NewsId,
                        principalTable: "News",
                        principalColumn: "NewsId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PageViews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "UserActivities",
                columns: table => new
                {
                    UserActivityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ActivityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RelatedNewsId = table.Column<int>(type: "int", nullable: true),
                    ActivityDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActivities", x => x.UserActivityId);
                    table.ForeignKey(
                        name: "FK_UserActivities_News_RelatedNewsId",
                        column: x => x.RelatedNewsId,
                        principalTable: "News",
                        principalColumn: "NewsId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_UserActivities_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyStats_Date",
                table: "DailyStats",
                column: "Date",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NewsAnalytics_NewsId",
                table: "NewsAnalytics",
                column: "NewsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PageViews_NewsId",
                table: "PageViews",
                column: "NewsId");

            migrationBuilder.CreateIndex(
                name: "IX_PageViews_UserId_NewsId_ViewDate",
                table: "PageViews",
                columns: new[] { "UserId", "NewsId", "ViewDate" });

            migrationBuilder.CreateIndex(
                name: "IX_UserActivities_ActivityType",
                table: "UserActivities",
                column: "ActivityType");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivities_RelatedNewsId",
                table: "UserActivities",
                column: "RelatedNewsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivities_UserId_ActivityDate",
                table: "UserActivities",
                columns: new[] { "UserId", "ActivityDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyStats");

            migrationBuilder.DropTable(
                name: "NewsAnalytics");

            migrationBuilder.DropTable(
                name: "PageViews");

            migrationBuilder.DropTable(
                name: "UserActivities");
        }
    }
}
