using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsPaper.Migrations
{
    /// <inheritdoc />
    public partial class fix_db_v0_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagesId",
                table: "News");

            migrationBuilder.AddColumn<string>(
                name: "ImagesLink",
                table: "News",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagesLink",
                table: "News");

            migrationBuilder.AddColumn<int>(
                name: "ImagesId",
                table: "News",
                type: "int",
                nullable: true);
        }
    }
}
