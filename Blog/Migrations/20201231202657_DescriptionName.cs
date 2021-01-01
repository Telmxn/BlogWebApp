using Microsoft.EntityFrameworkCore.Migrations;

namespace Blog.Migrations
{
    public partial class DescriptionName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Desctiprion",
                table: "Posts",
                newName: "Description");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Posts",
                newName: "Desctiprion");
        }
    }
}
