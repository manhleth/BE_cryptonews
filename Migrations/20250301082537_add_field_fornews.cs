using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsPaper.Migrations
{
    /// <inheritdoc />
    public partial class add_field_fornews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChildrenCategoryId",
                table: "News",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChildrenCategoryId",
                table: "News");
        }
    }
}
