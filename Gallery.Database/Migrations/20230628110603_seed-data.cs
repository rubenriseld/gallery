using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gallery.Database.Migrations
{
    /// <inheritdoc />
    public partial class seeddata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_ImageCollections_ImageCollectionId",
                table: "Images");

            migrationBuilder.AlterColumn<int>(
                name: "ImageCollectionId",
                table: "Images",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "ImageCollections",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "collection 1" });

            migrationBuilder.InsertData(
                table: "Images",
                columns: new[] { "Id", "Description", "ImageCollectionId", "ImageName", "Title" },
                values: new object[] { 1, "desc 1", 1, "test.png", "image 1" });

            migrationBuilder.AddForeignKey(
                name: "FK_Images_ImageCollections_ImageCollectionId",
                table: "Images",
                column: "ImageCollectionId",
                principalTable: "ImageCollections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_ImageCollections_ImageCollectionId",
                table: "Images");

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ImageCollections",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "ImageCollectionId",
                table: "Images",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_ImageCollections_ImageCollectionId",
                table: "Images",
                column: "ImageCollectionId",
                principalTable: "ImageCollections",
                principalColumn: "Id");
        }
    }
}
