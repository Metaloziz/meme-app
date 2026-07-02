using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MemeApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMemeImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Memes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Memes",
                type: "text",
                nullable: true);
        }
    }
}
