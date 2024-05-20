using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gallery.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedSaleAndDisplayProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShouldBeDisplayed",
                table: "ImageCollection",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Sold",
                table: "Image",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShouldBeDisplayed",
                table: "ImageCollection");

            migrationBuilder.DropColumn(
                name: "Sold",
                table: "Image");
        }
    }
}
