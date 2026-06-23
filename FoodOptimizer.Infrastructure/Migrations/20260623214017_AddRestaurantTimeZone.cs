using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodOptimizer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRestaurantTimeZone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_restaurants_restaurant_brands_BrandId",
                table: "restaurants");

            migrationBuilder.AlterColumn<long>(
                name: "BrandId",
                table: "restaurants",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "TimeZone",
                table: "restaurants",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_restaurants_restaurant_brands_BrandId",
                table: "restaurants",
                column: "BrandId",
                principalTable: "restaurant_brands",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_restaurants_restaurant_brands_BrandId",
                table: "restaurants");

            migrationBuilder.DropColumn(
                name: "TimeZone",
                table: "restaurants");

            migrationBuilder.AlterColumn<long>(
                name: "BrandId",
                table: "restaurants",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_restaurants_restaurant_brands_BrandId",
                table: "restaurants",
                column: "BrandId",
                principalTable: "restaurant_brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
