using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineOrderingSystem.Migrations
{
    /// <inheritdoc />
    public partial class productCommite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CommentsEnabled",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "HowMany",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommentsEnabled",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "HowMany",
                table: "Products");
        }
    }
}
