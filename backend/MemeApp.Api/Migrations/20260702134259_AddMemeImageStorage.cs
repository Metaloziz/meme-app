using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MemeApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMemeImageStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Memes",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "ImageContentType",
                table: "Memes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "Memes",
                type: "bytea",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageContentType",
                table: "Memes");

            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "Memes");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Memes",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
