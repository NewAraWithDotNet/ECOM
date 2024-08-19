using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineOrderingSystem.Migrations
{
    /// <inheritdoc />
    public partial class products : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discount_Categories_CategoryId",
                table: "Discount");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Discount",
                table: "Discount");

            migrationBuilder.RenameTable(
                name: "Discount",
                newName: "Discounts");

            migrationBuilder.RenameIndex(
                name: "IX_Discount_CategoryId",
                table: "Discounts",
                newName: "IX_Discounts_CategoryId");

            migrationBuilder.AddColumn<bool>(
                name: "ShowFlag",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Discounts",
                table: "Discounts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Discounts_Categories_CategoryId",
                table: "Discounts",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discounts_Categories_CategoryId",
                table: "Discounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Discounts",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "ShowFlag",
                table: "Products");

            migrationBuilder.RenameTable(
                name: "Discounts",
                newName: "Discount");

            migrationBuilder.RenameIndex(
                name: "IX_Discounts_CategoryId",
                table: "Discount",
                newName: "IX_Discount_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Discount",
                table: "Discount",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Discount_Categories_CategoryId",
                table: "Discount",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
