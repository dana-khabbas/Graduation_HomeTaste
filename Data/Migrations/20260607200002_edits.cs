using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace graduation_proj.Data.Migrations
{
    /// <inheritdoc />
    public partial class edits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_AspNetUsers_GuestId",
                table: "Reviews");

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
                name: "FK_Reviews_AspNetUsers_GuestId",
                table: "Reviews",
                column: "GuestId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Dishes_DishId1",
                table: "Reviews",
                column: "DishId1",
                principalTable: "Dishes",
                principalColumn: "DishId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_AspNetUsers_GuestId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Dishes_DishId1",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_DishId1",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "DishId1",
                table: "Reviews");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_AspNetUsers_GuestId",
                table: "Reviews",
                column: "GuestId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
