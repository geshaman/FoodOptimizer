using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FoodOptimizer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRestaurantBrand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BrandId",
                table: "restaurants",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "restaurant_brands",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_restaurant_brands", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_restaurants_BrandId",
                table: "restaurants",
                column: "BrandId");

            migrationBuilder.AddForeignKey(
                name: "FK_restaurants_restaurant_brands_BrandId",
                table: "restaurants",
                column: "BrandId",
                principalTable: "restaurant_brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_restaurants_restaurant_brands_BrandId",
                table: "restaurants");

            migrationBuilder.DropTable(
                name: "restaurant_brands");

            migrationBuilder.DropIndex(
                name: "IX_restaurants_BrandId",
                table: "restaurants");

            migrationBuilder.DropColumn(
                name: "BrandId",
                table: "restaurants");
        }
    }
}
