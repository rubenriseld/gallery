using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gallery.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedCoverImageAndCollectionOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverImageId",
                table: "ImageCollection",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoverImageCollectionId",
                table: "Image",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderInImageCollection",
                table: "Image",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ImageCollection_CoverImageId",
                table: "ImageCollection",
                column: "CoverImageId",
                unique: true,
                filter: "[CoverImageId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageCollection_Image_CoverImageId",
                table: "ImageCollection",
                column: "CoverImageId",
                principalTable: "Image",
                principalColumn: "ImageId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageCollection_Image_CoverImageId",
                table: "ImageCollection");

            migrationBuilder.DropIndex(
                name: "IX_ImageCollection_CoverImageId",
                table: "ImageCollection");

            migrationBuilder.DropColumn(
                name: "CoverImageId",
                table: "ImageCollection");

            migrationBuilder.DropColumn(
                name: "CoverImageCollectionId",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "OrderInImageCollection",
                table: "Image");
        }
    }
}
