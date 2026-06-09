using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace graduation_proj.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddJoinedAtAndIsApproved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "JoinedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 1, 1));

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Dishes",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JoinedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Dishes");
        }
    }
}
