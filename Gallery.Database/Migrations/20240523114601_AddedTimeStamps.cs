using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gallery.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedTimeStamps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Tag",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Tag",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "ImageCollection",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "ImageCollection",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Image",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Image",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ImageCollection");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "ImageCollection");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Image");
        }
    }
}
