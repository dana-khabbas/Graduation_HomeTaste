using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace graduation_proj.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixReviewDishRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove the extra shadow column — reviews already use DishId
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Dishes_DishId1",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_DishId1",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "DishId1",
                table: "Reviews");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DishId1",
                table: "Reviews",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_DishId1",
                table: "Reviews",
                column: "DishId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Dishes_DishId1",
                table: "Reviews",
                column: "DishId1",
                principalTable: "Dishes",
                principalColumn: "DishId");
        }
    }
}
