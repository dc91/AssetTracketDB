using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetTracketDB.Migrations
{
    /// <inheritdoc />
    public partial class AddOfficeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_Offices_OfficeId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "Office",
                table: "Asset");

            migrationBuilder.AlterColumn<int>(
                name: "OfficeId",
                table: "Asset",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_Offices_OfficeId",
                table: "Asset",
                column: "OfficeId",
                principalTable: "Offices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_Offices_OfficeId",
                table: "Asset");

            migrationBuilder.AlterColumn<int>(
                name: "OfficeId",
                table: "Asset",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Office",
                table: "Asset",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_Offices_OfficeId",
                table: "Asset",
                column: "OfficeId",
                principalTable: "Offices",
                principalColumn: "Id");
        }
    }
}
