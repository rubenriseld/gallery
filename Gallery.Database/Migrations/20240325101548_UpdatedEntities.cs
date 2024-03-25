using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gallery.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Image_ImageCollection_ImageCollectionId",
                table: "Image");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Image",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "ImageCollectionId",
                table: "Image",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Image_ImageCollection_ImageCollectionId",
                table: "Image",
                column: "ImageCollectionId",
                principalTable: "ImageCollection",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Image_ImageCollection_ImageCollectionId",
                table: "Image");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Image",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageCollectionId",
                table: "Image",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Image_ImageCollection_ImageCollectionId",
                table: "Image",
                column: "ImageCollectionId",
                principalTable: "ImageCollection",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
