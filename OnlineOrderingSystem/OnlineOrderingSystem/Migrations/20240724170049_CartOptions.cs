using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineOrderingSystem.Migrations
{
    /// <inheritdoc />
    public partial class CartOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "CartItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ProductOptionId",
                table: "CartItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductOptionId",
                table: "CartItems",
                column: "ProductOptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_ProductOptions_ProductOptionId",
                table: "CartItems",
                column: "ProductOptionId",
                principalTable: "ProductOptions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_ProductOptions_ProductOptionId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_ProductOptionId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "ProductOptionId",
                table: "CartItems");
        }
    }
}
