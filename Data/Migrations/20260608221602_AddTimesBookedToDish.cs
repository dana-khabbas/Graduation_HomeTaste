using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace graduation_proj.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTimesBookedToDish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TimesBooked",
                table: "Dishes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimesBooked",
                table: "Dishes");
        }
    }
}
